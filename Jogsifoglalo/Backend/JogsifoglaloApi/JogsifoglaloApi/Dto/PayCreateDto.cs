using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class PayCreateDto
    {
        [Required(ErrorMessage = "A felhasználó azonosítása kötelező.")]
        public int Felhasznalo_Id { get; set; }

        [Required(ErrorMessage = "Az időpont azonosítása kötelező.")]
        public required int Idopont_Id { get; set; }

        [Required(ErrorMessage = "Az összeg megadása kötelező.")]
        [Range(100, 100000, ErrorMessage = "Az összegnek 100 és 100.000 Ft között kell lennie.")]
        public required int Osszeg { get; set; }
    }
}
