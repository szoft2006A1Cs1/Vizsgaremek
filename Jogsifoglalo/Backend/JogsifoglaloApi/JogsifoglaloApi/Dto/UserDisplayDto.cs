namespace JogsifoglaloApi.Dto
{
    public class UserDisplayDto
    {
        public int Felhasznalo_Id { get; set; }
        public required string Nev { get; set; }
        public required string Email { get; set; }
        public required string Telefonszam { get; set; }
        public string? Cim { get; set; }
        public string Szerepkor { get; set; } = "tanulo";
    }
}
