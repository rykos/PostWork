using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace PostWork.Models
{
    public class Submission
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(50)")]
        public string Name { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }
        [Column(TypeName = "MEDIUMBLOB")]
        public byte[] Cv { get; set; }
        [Column(TypeName = "varchar(100)")]
        public string CvName { get; set; }
        public string Message { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime Date { get; set; } = DateTime.Now;

        public int PostId { get; set; }
        public Post Post { get; set; }
        [Column(TypeName = "varchar(250)")]

        public string UserId { get; set; }
        public SubmissionState State { get; set; }
    }

    public enum SubmissionState
    {
        None,
        Sent,
        Reviewing,
        Rejected,
        Accepted,
    }
}