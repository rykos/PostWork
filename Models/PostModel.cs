using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostWork.Models
{
    public class Post
    {
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string CreatorId { get; set; }
        [Required]
        [Column(TypeName = "varchar(150)")]
        public string Tags { get; set; }//Post tags, separated by [,]
        [Required]
        public string Description { get; set; }//Post description
        public byte[] Avatar { get; set; }//Image avatar as byte[]
        public int SalaryMin { get; set; }
        public int SalaryMax { get; set; }
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string Title { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }
}