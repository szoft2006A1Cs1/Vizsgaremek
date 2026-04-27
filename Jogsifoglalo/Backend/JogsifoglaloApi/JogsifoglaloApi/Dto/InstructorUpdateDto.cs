using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class InstructorUpdateDto
    {
        [MaxLength(100)]
        public string? Nev { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Phone]
        [MaxLength(20)]
        public string? Telefonszam { get; set; }

        [MaxLength(255)]
        public string? Cim { get; set; }

        [MaxLength(5, ErrorMessage = "Egy kategória neve nem lehet hosszabb 5 karakternél.")]
        public List<string>? UjKategoriak { get; set; }
    }
}
