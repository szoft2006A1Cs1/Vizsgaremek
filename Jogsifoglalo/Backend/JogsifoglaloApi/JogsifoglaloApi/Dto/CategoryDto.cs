using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class CategoryDto
    {
        public int Kategoria_Id { get; set; }
        public required string Kategoria_Kod { get; set; }
        public required string Kategoria_Nev { get; set; }

        [Range(1000, 50000, ErrorMessage = "Az óradíj 1000 és 50000 Ft között kell legyen.")]
        public required int Oradij {  get; set; }
        public required string Leiras { get; set; } 
    }
}
