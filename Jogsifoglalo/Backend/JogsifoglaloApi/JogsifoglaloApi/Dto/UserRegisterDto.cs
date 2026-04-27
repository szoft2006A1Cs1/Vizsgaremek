using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [MaxLength(100)]
        public required string Nev { get; set; }

        [Required(ErrorMessage = "Az email cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Nem megfelelő email formátum.")]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required(ErrorMessage = "A jelszó megadása kötelező.")]
        [MinLength(6, ErrorMessage = "A jelszónak legalább 6 karakternek kell lennie.")]
        public required string Jelszo { get; set; }

        [Required(ErrorMessage = "A telefonszám megadása kötelező.")]
        [Phone(ErrorMessage = "Nem megfelelő telefonszám formátum.")]
        [MaxLength(20)]
        public required string Telefonszam { get; set; }

        [MaxLength(255)]
        public string? Cim { get; set; }
    }
}
