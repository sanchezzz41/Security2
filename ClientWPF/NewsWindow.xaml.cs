using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Windows;
using Newtonsoft.Json;
using Security2.Dto.Models;
using Security2.Gronsfer;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for NewsWindow.xaml
    /// </summary>
    public partial class NewsWindow : Window
    {
        private HttpClient _httpClient;
        private readonly GronsfeldService _gronsfeldService;
        private readonly StorageSession _storage;


        public NewsWindow()
        {
            InitializeComponent();
            //Иницилизация сервиса для шифрования
            _gronsfeldService = new GronsfeldService(new GronfeldEncrypt(), new GronsfeldOptions
            {
                Alp = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=",
                KeyLenght = 20
            });
            //Иницилизация Http клиента
            _httpClient = new HttpClient(new HttpClientHandler {UseCookies = false})
            {
                BaseAddress = new Uri(CommonEnvironment.HostServer)
            };
            _storage = StorageSession.Create();
            var cookieName = StorageSession.SetCookieName;
            var cookie = _storage.Cookie.First();
            _httpClient.DefaultRequestHeaders.Add(cookieName, cookie);
            UpdateNewsList();
        }

        /// <summary>
        /// Метод для обновления новостей
        /// </summary>
        public async void UpdateNewsList()
        {
       
            var res = await _httpClient.GetAsync($"news");
            if (!res.IsSuccessStatusCode)
            {
                MessageBox.Show("Произогла ошибка.");
            }
            var content = await res.Content.ReadAsStringAsync();


            var afterDecrypt = _gronsfeldService.Decrypt(content, _storage.Key);

            var result = JsonConvert.DeserializeObject<List<NewsModel>>(afterDecrypt)
                .OrderByDescending(x=>x.CreatedDate)
                .Select(
                x=>$"Title:\t{x.Title}\nContent:{x.Content}\nTime:\t{x.CreatedDate}\n----------------");
            NewsList.Text = string.Join("\n", result);
        }

        /// <summary>
        /// метод для создания новости
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddNewsClick(object sender, RoutedEventArgs e)
        {
            var model = new NewsInfo
            {
                Content = Content.Text,
                Title = Title.Text
            };
            _httpClient.DefaultRequestHeaders.Add(StorageSession.SetCookieName, _storage.Cookie);
            var json = JsonConvert.SerializeObject(model);
            var encryptStr = _gronsfeldService.Encrypt(json, _storage.Key);
            var res = await _httpClient.PostAsync($"news?data={encryptStr}", null);
            var content = await res.Content.ReadAsStringAsync();
            var afterDecrypt = _gronsfeldService.Decrypt(content, _storage.Key);
            UpdateNewsList();
        }
    }
}
