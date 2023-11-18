using System.ComponentModel.DataAnnotations;

namespace API.DTO
{
    public class BaseUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }

    public class RegisterDTO : BaseUser
    {
        [Required]
        [StringLength(15, MinimumLength = 5)]
        public string Password { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }

    public class UserDTO : BaseUser
    {
        [Required]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }
    }

    public class AuthorizedUserDTO : UserDTO
    {
        [Required]
        public string Token { get; set; }
    }

}
