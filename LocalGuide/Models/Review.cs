namespace LocalGuide.Models
{
    /// <summary>
    /// Represents a user review for a specific location.
    /// Stores rating, textual feedback, and authorship details.
    /// </summary>
    public class Review
    {
        /// <summary>Gets or sets the unique identifier of the review.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the identifier of the reviewed location.</summary>
        public int LocationId { get; set; }

        /// <summary>Gets or sets the identifier of the user who wrote the review.</summary>
        public int UserId { get; set; }

        /// <summary>Gets or sets the rating given by the user (1 to 5).</summary>
        public int Rating { get; set; }

        /// <summary>Gets or sets the textual content of the review.</summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>Gets or sets the date and time when the review was created.</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>Gets or sets whether the review has been approved by a moderator.</summary>
        public bool IsApproved { get; set; } = false;

        /// <summary>Navigation property to the associated location.</summary>
        public Location? Location { get; set; }

        /// <summary>Navigation property to the author user.</summary>
        public User? Author { get; set; }
    }
}