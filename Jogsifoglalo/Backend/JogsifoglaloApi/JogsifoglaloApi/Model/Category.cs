using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JogsifoglaloApi.Model
{
    [Table("kategoriak")]
    public class Category
    {
        [Key]
        [Column("Kategoria_Id")]
        public int Kategoria_Id { get; set; }

        [Required, MaxLength(10)]
        [Column("Kategoria_Kod")]
        public required string Kategoria_Kod {  get; set; }

        [Required, MaxLength(100)]
        [Column("Kategoria_Nev")]
        public required string Kategoria_Nev { get; set; }

        [Required, Range(1000, 50000)]
        [Column("Oradij")]
        public required int Oradij { get; set; }

        [Required]
        [Column("Leiras", TypeName = "text")]
        public required string Leiras { get; set; }

        public ICollection<Instruct> Instructs { get; set; } = new List<Instruct>();
    }
}
