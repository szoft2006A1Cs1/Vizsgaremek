namespace JogsifoglaloApi.Dto
{
    public class AppointmentUpdateDto
    {
        public DateTime Kezdes_Dt { get; set; }
        public int Idotartam { get; set; }
        public decimal Ar { get; set; }
        public string? Megjegyzes { get; set; }
    }
}
