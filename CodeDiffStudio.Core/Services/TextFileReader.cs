namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Reads text files without modifying their contents.
    /// </summary>
    public class TextFileReader : ITextFileReader
    {
        /// <summary>
        /// Reads a file and captures its path and text.
        /// </summary>
        /// <param name="filePath">The path of the file to read.</param>
        /// <param name="cancellationToken">The token used to request cancellation.</param>
        /// <returns>A task containing the successfully loaded file snapshot.</returns>
        public async Task<FileSnapshot> ReadAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

            string text = await System.IO.File.ReadAllTextAsync(
                filePath,
                cancellationToken);

            return new FileSnapshot(filePath, text);
        }
    }
}
