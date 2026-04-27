namespace JogsifoglaloApi.Dto
{
    public class PaymentAdminResponseDto
    {
        public int Fizetes_Id { get; set; }
        public required string VasarloNeve { get; set; }
        public int Osszeg { get; set; }
        public DateTime Datum { get; set; }
        public int Idopont_Id { get; set; }
    }
}
