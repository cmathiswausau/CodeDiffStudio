namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Represents a difference between two lines.
    /// </summary>
    public class LineDifference
    {
        /// <summary>
        /// Gets or sets the type of difference.
        /// </summary>
        public DifferenceType DifferenceType { get; set; }

        /// <summary>
        /// Gets or sets the line number in the second file.
        /// </summary>
        public int NewLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the content of the line in the second file.
        /// </summary>
        public string? NewText { get; set; }

        /// <summary>
        /// Gets or sets the line number in the first file.
        /// </summary>
        public int OriginalLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the content of the line in the first file.
        /// </summary>
        public string? OriginalText { get; set; }
    }
}
