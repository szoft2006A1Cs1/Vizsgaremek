namespace JogsifoglaloApi.Dto
{
    public class UserTokenDto
    {
        public required string Token { get; set; }
        public int Felhasznalo_Id { get; set; }
        public required string Nev { get; set; }
        public string Szerepkor { get; set; } = "tanulo";
    }
}
