using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("oktat")]
    public class Instruct
    {
        [Key]
        [Column("Oktat_Id")]
        public int Oktat_Id { get; set; }

        [Required]
        [Column("Oktato_Id")]
        public required int Oktato_Id { get; set; }

        [ForeignKey("Oktato_Id")]
        public virtual Instructor? Oktato { get; set; }

        [Required]
        [Column("Kategoria_Id")]
        public required int Kategoria_Id { get; set; }

        [ForeignKey("Kategoria_Id")]
        public virtual Category? Kategoria { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
