namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Defines read-only access to text files.
    /// </summary>
    public interface ITextFileReader
    {
        /// <summary>
        /// Reads a file and captures its path and text.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="cancellationToken">The token used to request cancellation.</param>
        /// <returns>A task containing the successfully loaded file snapshot.</returns>
        Task<FileSnapshot> ReadAsync(
            string filePath,
            CancellationToken cancellationToken = default);
    }
}
