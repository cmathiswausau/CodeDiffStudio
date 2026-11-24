namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Reads text files without modifying their contents.
    /// </summary>
    public class TextFileReader : ITextFileReader
    {
        /// <summary>
        /// Reads the complete text of a file asynchronously.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="cancellationToken">The token used to request cancellation.</param>
        /// <returns>A task containing the file text.</returns>
        public Task<string> ReadAllTextAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            return System.IO.File.ReadAllTextAsync(
                filePath,
                cancellationToken);
        }
    }
}
