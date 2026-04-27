using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Az email cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Nem megfelelő email formátum.")]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        public required string Jelszo { get; set; }
    }
}
