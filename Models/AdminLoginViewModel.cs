using System.ComponentModel.DataAnnotations;

namespace KulturAtlasi.ViewModels
{
    public class AdminLoginViewModel
    {
        [Required(ErrorMessage = "Email adresi zorunludur.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}