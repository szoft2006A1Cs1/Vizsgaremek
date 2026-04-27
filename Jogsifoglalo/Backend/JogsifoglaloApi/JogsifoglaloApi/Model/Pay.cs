using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("fizetesek")]
    public class Pay
    {
        [Key]
        [Column("Fizetes_Id")]
        public int Fizetes_Id { get; set; }

        [Required]
        [Column("Felhasznalo_Id")]
        public required int Felhasznalo_Id { get; set; }

        [ForeignKey("Felhasznalo_Id")]
        public virtual User? Felhasznalo { get; set; }

        [Required]
        [Column("Idopont_Id")]
        public required int Idopont_Id { get; set; }

        [ForeignKey("Idopont_Id")]
        public virtual Appointment? Idopont { get; set; }

        [Required]
        [Column("Osszeg")]
        public required int Osszeg { get; set; }

        [Required]
        [Column("Datum", TypeName = "datetime")]
        public required DateTime Datum { get; set; }

    }
}
