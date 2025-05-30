namespace Daikon.Shared.Embedded.DocuStore
{
    /// <summary>
    /// Represents a user-submitted rating with optional comments.
    /// </summary>
    public class Rating
    {
        /// <summary>
        /// Unique identifier for the rating entry.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ID of the user who submitted the rating.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Numeric score, from 0 (worst) to 5 (best).
        /// </summary>
        public int Score
        {
            get => _score;
            set
            {
                if (value < 0 || value > 5)
                    throw new ArgumentOutOfRangeException(nameof(Score), "Score must be between 0 and 5.");
                _score = value;
            }
        }
        private int _score;

        /// <summary>
        /// Optional user comment explaining the rating.
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// The date and time when the rating was given.
        /// </summary>
        public DateTime RatedAt { get; set; } = DateTime.UtcNow;
    }
}
