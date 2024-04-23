using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Recipe_Site.Models
{
    public class Users
    {
        [Key]
        [DataType(DataType.EmailAddress)]
        public string EmailId { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [MaxLength(30)]
        public string Fname { get; set; }
        [MaxLength(30)]
        public string Lname { get; set; }

        [Column(TypeName = "Date")]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }

        public string ProfilePic { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
        public int OTP { get; set; }
    }
}
