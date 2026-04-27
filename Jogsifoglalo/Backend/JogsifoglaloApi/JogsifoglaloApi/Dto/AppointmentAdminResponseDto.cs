namespace JogsifoglaloApi.Dto
{
    public class AppointmentAdminResponseDto
    {
        public int Idopont_Id { get; set; }
        public required string OktatoNeve { get; set; } 
        public string? TanuloNeve { get; set; }  
        public DateTime Kezdes_Dt { get; set; }
        public int Idotartam { get; set; }
        public int Ar { get; set; }
        public required string Allapot { get; set; }   
        public string? Megjegyzes { get; set; }
    }
}
