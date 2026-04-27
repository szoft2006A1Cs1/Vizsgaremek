using System.ComponentModel.DataAnnotations;

namespace JogsifoglaloApi.Dto
{
    public class AppointmentDisplayDto
    {
            public int Idopont_Id { get; set; }
            public DateTime Kezdes_Dt { get; set; }
            public int Idotartam { get; set; }
            public int Ar { get; set; }
            public string? Oktato_Nev { get; set; }
            public string? Tanulo_Nev { get; set; }
            public string? Kategoria_Nev { get; set; }
            public string? Kategoria_Kod { get; set; }
            public string? Allapot { get; set; }
            public string? Megjegyzes { get; set; }
    }
}
