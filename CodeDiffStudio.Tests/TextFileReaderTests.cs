namespace CodeDiffStudio.Tests
{
    /// <summary>
    /// Contains tests for read-only text file loading.
    /// </summary>
    [TestClass]
    public sealed class TextFileReaderTests
    {
        /// <summary>
        /// Verifies that a cancellation request is passed to the file read.
        /// </summary>
        /// <returns>A task representing the asynchronous test.</returns>
        [TestMethod]
        public async Task CancelledReadThrowsOperationCanceledException()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.txt");

            try
            {
                await File.WriteAllTextAsync(filePath, "Cancellation fixture");

                using CancellationTokenSource cancellationSource =
                    new CancellationTokenSource();

                cancellationSource.Cancel();

                CodeDiffStudio.Core.ITextFileReader reader =
                    new CodeDiffStudio.Core.TextFileReader();

                await Assert.ThrowsAsync<OperationCanceledException>(
                    () => reader.ReadAllTextAsync(
                        filePath,
                        cancellationSource.Token));
            }
            finally
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Verifies that a missing file produces a file-not-found error.
        /// </summary>
        /// <returns>A task representing the asynchronous test.</returns>
        [TestMethod]
        public async Task MissingFileThrowsFileNotFoundException()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.txt");

            CodeDiffStudio.Core.ITextFileReader reader =
                new CodeDiffStudio.Core.TextFileReader();

            await Assert.ThrowsExactlyAsync<FileNotFoundException>(
                () => reader.ReadAllTextAsync(filePath));
        }

        /// <summary>
        /// Verifies exact text preservation without changing source bytes.
        /// </summary>
        /// <param name="expectedText">The text to write and read.</param>
        /// <returns>A task representing the asynchronous test.</returns>
        [TestMethod]
        [DataRow("")]
        [DataRow("Café 日本語 😀")]
        [DataRow("First\r\nSecond\nThird\rLast")]
        [DataRow("First\r\nSecond\r\n")]
        public async Task ReadingPreservesTextAndSourceBytes(string expectedText)
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.txt");

            byte[] originalBytes =
                new System.Text.UTF8Encoding(false).GetBytes(expectedText);

            try
            {
                await File.WriteAllBytesAsync(filePath, originalBytes);

                CodeDiffStudio.Core.ITextFileReader reader =
                    new CodeDiffStudio.Core.TextFileReader();

                string actualText = await reader.ReadAllTextAsync(filePath);
                byte[] remainingBytes = await File.ReadAllBytesAsync(filePath);

                Assert.AreEqual(expectedText, actualText);
                CollectionAssert.AreEqual(originalBytes, remainingBytes);
            }
            finally
            {
                File.Delete(filePath);
            }
        }
    }
}
