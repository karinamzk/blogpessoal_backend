﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace blogpessoal.Model
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column(TypeName = "Varchar")]
        [StringLength(255)]
        public string Nome { get; set; } = string.Empty;

        [Column(TypeName = "Varchar")]
        [StringLength(255)]
        public string Usuario { get; set; } = string.Empty;

        [Column(TypeName = "Varchar")]
        [StringLength(255)]
        public string Senha { get; set; } = string.Empty;

        [Column(TypeName = "Varchar")]
        [StringLength(5000)]
        public string? Foto { get; set; } = string.Empty;

        [InverseProperty("Usuario")]
        
        public virtual ICollection<Postagem>? Postagem { get; set;}

    }
}
