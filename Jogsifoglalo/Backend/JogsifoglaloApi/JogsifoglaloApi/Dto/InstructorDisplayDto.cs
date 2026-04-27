namespace JogsifoglaloApi.Dto
{
    public class InstructorDisplayDto
    {
        public int Oktato_Id { get; set; }
        public required string Nev {  get; set; }
        public required string Email { get; set; }
        public required string Telefonszam { get; set; }
        public string? Cim { get; set; }
        public List<string> Kategoriak { get; set; } = new();
    }
}
