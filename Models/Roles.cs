using System.ComponentModel.DataAnnotations;
namespace Live_Quiz.Models
{

    public class Roles
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [Display(Name = "Role")]
        public string Name { get; set; }
    }
    public class UserRoles
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string RoleId { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}