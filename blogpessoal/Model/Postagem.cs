using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace blogpessoal.Model
{
    public class Postagem : Auditable
    {
        [Key] // Primary Keu (Id)
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // IDENTITY (1,1)
        public long Id {get;set;}

        [Column(TypeName = "Varchar")]
        [StringLength(100)]
        public string Titulo { get;set;} = string.Empty;

        [Column(TypeName = "Varchar")]
        [StringLength(1000)]
        public string Texto { get; set; } = string.Empty;

        public virtual Tema? Tema { get; set; }
        
    }
}
