using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Security2.Dto.Models;
using Security2.Rsa;

namespace ClientWPF
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        private HttpClient _httpClient;
        private RsaService _rsaService;
        public RegistrationWindow()
        {
            InitializeComponent();
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(CommonEnvironment.HostServer)
            };

            _rsaService = new RsaService();
        }

        private async void Registration(object sender, RoutedEventArgs e)
        {
            var serverPublicKey = await GetPubKey();
            var model = new UserRegisterWithTimePassword(Email.Text, Name.Text,
                new TimePasswordInfo(Pass1.Text, Pass2.Text, Pass3.Text, Pass4.Text, Pass5.Text, Pass6.Text,
                    Pass7.Text));

            model.TimePasswords.FirstPass = _rsaService.Encrypt(model.TimePasswords.FirstPass, serverPublicKey);
            model.TimePasswords.SecondPass = _rsaService.Encrypt(model.TimePasswords.SecondPass, serverPublicKey);
            model.TimePasswords.ThreePass = _rsaService.Encrypt(model.TimePasswords.ThreePass, serverPublicKey);
            model.TimePasswords.FourPass = _rsaService.Encrypt(model.TimePasswords.FourPass, serverPublicKey);
            model.TimePasswords.FivePass = _rsaService.Encrypt(model.TimePasswords.FivePass, serverPublicKey);
            model.TimePasswords.SixPass = _rsaService.Encrypt(model.TimePasswords.SixPass, serverPublicKey);
            model.TimePasswords.SevenPass = _rsaService.Encrypt(model.TimePasswords.SevenPass, serverPublicKey);

            model.Email = _rsaService.Encrypt(model.Email, serverPublicKey);
            model.Name = _rsaService.Encrypt(model.Name, serverPublicKey);

            var responseGuid = await _httpClient.PostAsJsonAsync("TimePassword/Register", model);
            responseGuid.EnsureSuccessStatusCode();
            MessageBox.Show($"Ваш Guid:{await responseGuid.Content.ReadAsAsync<Guid>()}");
            Thread.Sleep(TimeSpan.FromSeconds(2));
            this.Close();
        }

        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async Task<RsaPublicKey> GetPubKey()
        {
            var responsePubKey = await _httpClient.GetAsync("rsa/RsaPublicKey");
            responsePubKey.EnsureSuccessStatusCode();

            return await responsePubKey.Content.ReadAsAsync<RsaPublicKey>();
        }
    }
}
