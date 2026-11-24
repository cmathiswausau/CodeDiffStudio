namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Defines read-only access to text files.
    /// </summary>
    public interface ITextFileReader
    {
        /// <summary>
        /// Reads the complete text of a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="cancellationToken">The token used to request cancellation.</param>
        /// <returns>A task containing the file text.</returns>
        Task<string> ReadAllTextAsync(
            string filePath,
            CancellationToken cancellationToken = default);
    }
}
