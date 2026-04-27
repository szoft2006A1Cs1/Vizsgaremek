using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class AppointmentCreateDto
    {
        [Required(ErrorMessage = "Az oktatási azonosító megadása kötelező.")]
        public int Oktat_Id { get; set; }

        [Required(ErrorMessage = "A kezdési időpont megadása kötelező.")]
        public DateTime Kezdes_Dt { get; set; }

        [Required(ErrorMessage = "Az időtartam megadása kötelező.")]
        [Range(30, 240, ErrorMessage = "Az óra időtartama 30 és 240 perc között kell legyen.")]
        public int Idotartam { get; set; }

        [Required(ErrorMessage = "Az ár megadása kötelező.")]
        [Range(0, 100000, ErrorMessage = "Az ár nem lehet negatív és nem haladhatja meg a 100.000 Ft-ot.")]
        public int Ar { get; set; }

        [StringLength(255, ErrorMessage = "A megjegyzés legfeljebb 255 karakter hosszú lehet.")]
        public string? Megjegyzes { get; set; }
    }
}
