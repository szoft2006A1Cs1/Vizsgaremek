namespace JogsifoglaloApi.Dto
{
    public class UserAdminResponseDto
    {
        public int Felhasznalo_Id { get; set; }
        public required string Nev { get; set; }
        public string? Cim { get; set; }
        public required string Email { get; set; }
        public required string Telefonszam { get; set; }
        public required string Szerepkor_Nev { get; set; }
    }
}
