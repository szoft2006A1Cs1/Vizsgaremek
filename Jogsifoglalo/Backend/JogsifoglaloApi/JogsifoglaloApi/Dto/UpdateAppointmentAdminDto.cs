namespace JogsifoglaloApi.Dto
{
    public class UpdateAppointmentAdminDto
    {
        public DateTime Kezdes_Dt { get; set; }
        public ushort Idotartam { get; set; }
        public int Ar { get; set; }
        public string Allapot { get; set; } = string.Empty;
        public string? Megjegyzes { get; set; }
    }
}
