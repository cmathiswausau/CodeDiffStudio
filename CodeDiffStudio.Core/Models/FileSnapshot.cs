namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Contains a file's path and text from a successful read.
    /// </summary>
    public sealed class FileSnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSnapshot" /> class.
        /// </summary>
        /// <param name="filePath">The path of the file that was read.</param>
        /// <param name="text">The text read from the file.</param>
        public FileSnapshot(string filePath, string text)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
            ArgumentNullException.ThrowIfNull(text);

            this.FilePath = filePath;
            this.Text = text;
        }

        /// <summary>
        /// Gets the path of the file that was read.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Gets the text read from the file.
        /// </summary>
        public string Text { get; }
    }
}
