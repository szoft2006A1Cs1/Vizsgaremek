using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("oktatok")]
    public class Instructor
    {
        [Key]
        [Column("Oktato_Id")]
        public int Oktato_Id { get; set; }

        [Required]
        [Column("Felhasznalo_Id")]
        public required int Felhasznalo_Id { get; set; }

        [ForeignKey("Felhasznalo_Id")]
        public virtual User? Felhasznalo { get; set; }

        public virtual ICollection<Instruct> Instructs { get; set; } = new List<Instruct>();
    }
}
