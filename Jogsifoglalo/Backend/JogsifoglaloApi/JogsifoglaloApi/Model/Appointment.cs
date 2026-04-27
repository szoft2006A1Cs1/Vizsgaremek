using JogsifoglaloApi.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("idopontok")]
    public class Appointment
    {
        [Key]
        [Column("Idopont_Id")]
        public int Idopont_Id { get; set; }

        [Required]
        [Column("Oktat_Id")]
        public required int Oktat_Id { get; set; }

        [ForeignKey("Oktat_Id")]
        public virtual Instruct? Oktat { get; set; }

        [Column("Tanulo_Id")]
        public int? Tanulo_Id { get; set; }

        [ForeignKey("Tanulo_Id")]
        [InverseProperty("BookedAppointments")]
        public virtual User? Tanulo { get; set; }

        [Required]
        [Column("Kezdes_Dt", TypeName = "datetime")]
        public required DateTime Kezdes_Dt { get; set; }

        [Required]
        [Column("Idotartam", TypeName = "smallint unsigned")]
        public ushort Idotartam { get; set; } = 120;

        [Required]
        [Column("Ar")]
        public required int Ar { get; set; }

        [MaxLength(255)]
        [Column("Megjegyzes")]
        public string? Megjegyzes { get; set; }

        public virtual Pay? Pay { get; set; }

        [Required]
        [Column("Allapot", TypeName = "enum('szabad','foglalt','lemondva,'teljesitve'')")]
        public Status Allapot { get; set; } = Status.szabad;

    }
}
