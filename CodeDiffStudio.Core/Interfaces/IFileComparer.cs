namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Defines exact text comparison without modifying source files.
    /// </summary>
    public interface IFileComparer
    {
        /// <summary>
        /// Compares text while preserving differences in line endings.
        /// </summary>
        /// <param name="originalText">The original file text.</param>
        /// <param name="newText">The new file text.</param>
        /// <param name="cancellationToken">The token used to request cancellation.</param>
        /// <returns>The completed comparison results.</returns>
        /// <exception cref="OperationCanceledException">
        /// Cancellation was requested before the comparison completed.
        /// </exception>
        ComparisonResults CompareText(
            string originalText,
            string newText,
            CancellationToken cancellationToken = default);
    }
}
