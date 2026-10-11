using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class UserProfile
    {
       [Key]
        public int ProfileID { get; set; }

        public int UserID { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [MaxLength(255)]
        public string? Address { get; set; }

        [MaxLength(255)]
        public string? ProfilePictureURL { get; set; }

        public User? User { get; set; }
    }
}