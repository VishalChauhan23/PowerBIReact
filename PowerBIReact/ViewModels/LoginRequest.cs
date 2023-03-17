using System.ComponentModel.DataAnnotations;

namespace PowerBIReact.ViewModels
{
    public class LoginRequest
    {
        [Display(Name = "Email")]
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
