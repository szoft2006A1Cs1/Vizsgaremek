using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class UpdateUserAdminDto
    {
        [Required(ErrorMessage = "A név kötelező!")]
        public required string Nev { get; set; }

        public string? Cim { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Nem érvényes email cím!")]
        public required string Email { get; set; }

        [Required]
        public required string Telefonszam { get; set; }

        [Required]
        public required string Szerepkor_Nev { get; set; }
    }
}
