namespace CodeDiffStudio.Tests
{
    /// <summary>
    /// Contains tests for the comparison cancellation contract.
    /// </summary>
    [TestClass]
    public sealed class FileComparerCancellationTests
    {
        /// <summary>
        /// Verifies that cancelled line comparison does not return a result.
        /// </summary>
        [TestMethod]
        public void CancelledLineComparisonThrowsOperationCanceledException()
        {
            using CancellationTokenSource cancellationSource =
                new CancellationTokenSource();

            cancellationSource.Cancel();

            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "Original" };
            string[] newLines = { "Changed" };

            Assert.ThrowsExactly<OperationCanceledException>(
                () => comparer.Compare(
                    originalLines,
                    newLines,
                    cancellationSource.Token));
        }

        /// <summary>
        /// Verifies that cancelled text comparison rejects even empty inputs.
        /// </summary>
        [TestMethod]
        public void CancelledTextComparisonThrowsOperationCanceledException()
        {
            using CancellationTokenSource cancellationSource =
                new CancellationTokenSource();

            cancellationSource.Cancel();

            CodeDiffStudio.Core.IFileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            Assert.ThrowsExactly<OperationCanceledException>(
                () => comparer.CompareText(
                    string.Empty,
                    string.Empty,
                    cancellationSource.Token));
        }

        /// <summary>
        /// Verifies exact line-ending comparison with an uncancelled token.
        /// </summary>
        [TestMethod]
        public void UncancelledComparisonPreservesLineEndingDifferences()
        {
            using CancellationTokenSource cancellationSource =
                new CancellationTokenSource();

            CodeDiffStudio.Core.IFileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.CompareText(
                    "First\r\nLast",
                    "First\nLast",
                    cancellationSource.Token);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual("First\r\n", results.LineDifferences[0].OriginalText);
            Assert.AreEqual("First\n", results.LineDifferences[0].NewText);
        }
    }
}
