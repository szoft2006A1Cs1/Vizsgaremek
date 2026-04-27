using JogsifoglaloApi.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("felhasznalok")]
    public class User
    {
        [Key]
        [Column("Felhasznalo_Id")]
        public int Felhasznalo_Id { get; set; }

        [Required, MaxLength(100)]
        [Column("Nev")]
        public required string Nev { get; set; }

        [MaxLength(100)]
        [Column("Cim")]
        public string? Cim { get; set; }

        [Required, EmailAddress, MaxLength(100)]
        [Column("Email")]
        public required string Email { get; set; }

        [Required, MaxLength(255)]
        [Column("Jelszo")]
        public required string Jelszo { get; set; }

        [Required, Phone, MaxLength(20)]
        [Column("Telefonszam")]
        public required string Telefonszam { get; set; }

        [Required]
        [Column("Szerepkor_Nev", TypeName = "enum('tanulo','oktato','admin')")]
        public Role Szerepkor_Nev { get; set; } = Role.tanulo;

        public virtual ICollection<Pay> Pays { get; set; } = new List<Pay>();

        public virtual Instructor? Instructors { get; set; }

        [InverseProperty("Tanulo")]
        public virtual ICollection<Appointment> BookedAppointments { get; set; } = new List<Appointment>();
    }
}
