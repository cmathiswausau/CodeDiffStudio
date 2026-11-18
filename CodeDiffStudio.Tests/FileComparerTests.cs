namespace CodeDiffStudio.Tests
{
    /// <summary>
    /// Contains tests for comparing file lines.
    /// </summary>
    [TestClass]
    public sealed class FileComparerTests
    {
        /// <summary>
        /// Verifies that different line endings produce a difference.
        /// </summary>
        [TestMethod]
        public void DifferentLineEndingsAreModified()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string originalText = "First line\r\nSecond line";
            string newText = "First line\nSecond line";

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.CompareText(originalText, newText);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(2, results.LineDifferences.Count);

            CodeDiffStudio.Core.LineDifference line =
                results.LineDifferences[0];

            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Modified,
                line.DifferenceType);
            Assert.AreEqual("First line\r\n", line.OriginalText);
            Assert.AreEqual("First line\n", line.NewText);
        }

        /// <summary>
        /// Verifies that different text at the same position is modified.
        /// </summary>
        [TestMethod]
        public void DifferentLinesAreModified()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First line", "Old text" };
            string[] newLines = { "First line", "New text" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(2, results.LineDifferences.Count);

            CodeDiffStudio.Core.LineDifference line =
                results.LineDifferences[1];

            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Modified,
                line.DifferenceType);
            Assert.AreEqual(2, line.OriginalLineNumber);
            Assert.AreEqual(2, line.NewLineNumber);
            Assert.AreEqual("Old text", line.OriginalText);
            Assert.AreEqual("New text", line.NewText);
        }

        /// <summary>
        /// Verifies that comparing two empty arrays produces no line differences.
        /// </summary>
        [TestMethod]
        public void EmptyFilesHaveNoDifferences()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = new string[0];
            string[] newLines = new string[0];

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.IsTrue(results.AreIdentical);
            Assert.AreEqual(0, results.DifferenceCount);
            Assert.AreEqual(0, results.LineDifferences.Count);
        }

        /// <summary>
        /// Verifies that an extra line in the new file is added.
        /// </summary>
        [TestMethod]
        public void ExtraNewLineIsAdded()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First line" };
            string[] newLines = { "First line", "Added text" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(2, results.LineDifferences.Count);

            CodeDiffStudio.Core.LineDifference line =
                results.LineDifferences[1];

            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Added,
                line.DifferenceType);
            Assert.AreEqual(0, line.OriginalLineNumber);
            Assert.AreEqual(2, line.NewLineNumber);
            Assert.IsNull(line.OriginalText);
            Assert.AreEqual("Added text", line.NewText);
        }

        /// <summary>
        /// Verifies that an extra line in the original file is removed.
        /// </summary>
        [TestMethod]
        public void ExtraOriginalLineIsRemoved()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First line", "Removed text" };
            string[] newLines = { "First line" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(2, results.LineDifferences.Count);

            CodeDiffStudio.Core.LineDifference line =
                results.LineDifferences[1];

            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Removed,
                line.DifferenceType);
            Assert.AreEqual(2, line.OriginalLineNumber);
            Assert.AreEqual(0, line.NewLineNumber);
            Assert.AreEqual("Removed text", line.OriginalText);
            Assert.IsNull(line.NewText);
        }

        /// <summary>
        /// Verifies that an inserted line preserves the following matching line.
        /// </summary>
        [TestMethod]
        public void InsertedMiddleLinePreservesFollowingMatch()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First", "Last" };
            string[] newLines = { "First", "Inserted", "Last" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(3, results.LineDifferences.Count);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Added,
                results.LineDifferences[1].DifferenceType);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Unchanged,
                results.LineDifferences[2].DifferenceType);
            Assert.AreEqual(2, results.LineDifferences[2].OriginalLineNumber);
            Assert.AreEqual(3, results.LineDifferences[2].NewLineNumber);
        }

        /// <summary>
        /// Verifies that an insertion between repeated lines preserves their matches.
        /// </summary>
        [TestMethod]
        public void InsertionBetweenRepeatedLinesPreservesMatches()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "Repeat", "Repeat", "Last" };
            string[] newLines = { "Repeat", "Inserted", "Repeat", "Last" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(4, results.LineDifferences.Count);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Added,
                results.LineDifferences[1].DifferenceType);
            Assert.AreEqual("Inserted", results.LineDifferences[1].NewText);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Unchanged,
                results.LineDifferences[2].DifferenceType);
            Assert.AreEqual(2, results.LineDifferences[2].OriginalLineNumber);
            Assert.AreEqual(3, results.LineDifferences[2].NewLineNumber);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Unchanged,
                results.LineDifferences[3].DifferenceType);
        }

        /// <summary>
        /// Verifies that matching lines are unchanged.
        /// </summary>
        [TestMethod]
        public void MatchingLinesAreUnchanged()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First line", "Second line" };
            string[] newLines = { "First line", "Second line" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.IsTrue(results.AreIdentical);
            Assert.AreEqual(0, results.DifferenceCount);
            Assert.AreEqual(2, results.LineDifferences.Count);

            for (int index = 0; index < originalLines.Length; index++)
            {
                CodeDiffStudio.Core.LineDifference line =
                    results.LineDifferences[index];

                Assert.AreEqual(
                    CodeDiffStudio.Core.DifferenceType.Unchanged,
                    line.DifferenceType);
                Assert.AreEqual(index + 1, line.OriginalLineNumber);
                Assert.AreEqual(index + 1, line.NewLineNumber);
                Assert.AreEqual(originalLines[index], line.OriginalText);
                Assert.AreEqual(newLines[index], line.NewText);
            }
        }

        /// <summary>
        /// Verifies that matching text preserves all supported line endings.
        /// </summary>
        [TestMethod]
        public void MatchingTextPreservesLineEndings()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string text = "First\r\nSecond\nThird\rFourth";

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.CompareText(text, text);

            Assert.IsTrue(results.AreIdentical);
            Assert.AreEqual(0, results.DifferenceCount);
            Assert.AreEqual(4, results.LineDifferences.Count);

            string[] expectedLines = { "First\r\n", "Second\n", "Third\r", "Fourth" };

            for (int index = 0; index < expectedLines.Length; index++)
            {
                CodeDiffStudio.Core.LineDifference line =
                    results.LineDifferences[index];

                Assert.AreEqual(
                    CodeDiffStudio.Core.DifferenceType.Unchanged,
                    line.DifferenceType);
                Assert.AreEqual(expectedLines[index], line.OriginalText);
                Assert.AreEqual(expectedLines[index], line.NewText);
                Assert.AreEqual(index + 1, line.OriginalLineNumber);
                Assert.AreEqual(index + 1, line.NewLineNumber);
            }
        }

        /// <summary>
        /// Verifies that a missing final newline produces a difference.
        /// </summary>
        [TestMethod]
        public void MissingFinalNewlineIsModified()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string originalText = "First line\n";
            string newText = "First line";

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.CompareText(originalText, newText);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(1, results.LineDifferences.Count);

            CodeDiffStudio.Core.LineDifference line =
                results.LineDifferences[0];

            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Modified,
                line.DifferenceType);
            Assert.AreEqual("First line\n", line.OriginalText);
            Assert.AreEqual("First line", line.NewText);
        }

        /// <summary>
        /// Verifies that a removed line preserves the following matching line.
        /// </summary>
        [TestMethod]
        public void RemovedMiddleLinePreservesFollowingMatch()
        {
            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            string[] originalLines = { "First", "Removed", "Last" };
            string[] newLines = { "First", "Last" };

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.Compare(originalLines, newLines);

            Assert.AreEqual(1, results.DifferenceCount);
            Assert.AreEqual(3, results.LineDifferences.Count);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Removed,
                results.LineDifferences[1].DifferenceType);
            Assert.AreEqual("Removed", results.LineDifferences[1].OriginalText);
            Assert.IsNull(results.LineDifferences[1].NewText);
            Assert.AreEqual(
                CodeDiffStudio.Core.DifferenceType.Unchanged,
                results.LineDifferences[2].DifferenceType);
            Assert.AreEqual(3, results.LineDifferences[2].OriginalLineNumber);
            Assert.AreEqual(2, results.LineDifferences[2].NewLineNumber);
        }
    }
}
