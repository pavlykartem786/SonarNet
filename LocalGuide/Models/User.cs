namespace LocalGuide.Models
{
    /// <summary>
    /// Represents a registered user of the LocalGuide platform.
    /// Stores credentials, profile data, and role information.
    /// </summary>
    public class User
    {
        /// <summary>Gets or sets the unique identifier of the user.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the user's email address (used as login).</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Gets or sets the bcrypt-hashed password.</summary>
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>Gets or sets the display name shown in reviews.</summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>Gets or sets the role of the user.</summary>
        public UserRole Role { get; set; } = UserRole.Visitor;

        /// <summary>Gets or sets the date when the user registered.</summary>
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets whether the email has been confirmed.</summary>
        public bool IsEmailConfirmed { get; set; } = false;

        /// <summary>Gets or sets the list of reviews authored by this user.</summary>
        public List<Review> Reviews { get; set; } = new();
    }

    /// <summary>Defines user roles for authorization.</summary>
    public enum UserRole
    {
        /// <summary>Regular visitor — can read and write reviews.</summary>
        Visitor,
        /// <summary>Administrator — full management access.</summary>
        Admin
    }
}