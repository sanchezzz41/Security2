namespace Security2.Dto.Models
{
    /// <summary>
    /// Модель для пердачи служебной инфы
    /// </summary>
    public class ServiceInfo
    {
        public string Password { get; set; }

        /// <summary>
        /// Симетричный ключ
        /// </summary>
        public string Key { get; set; }

        public string Email { get; set; }
    }
}