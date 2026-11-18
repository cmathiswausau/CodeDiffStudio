namespace CodeDiffStudio.Tests
{
    /// <summary>
    /// Contains unit tests for CodeDiff Studio.
    /// </summary>
    [TestClass]
    public sealed class ComparisonResultsTests
    {
        /// <summary>
        /// Verifies that an empty comparison has no differences.
        /// </summary>
        [TestMethod]
        public void EmptyComparisonHasNoDifferences()
        {
            CodeDiffStudio.Core.ComparisonResults results =
                new CodeDiffStudio.Core.ComparisonResults();

            Assert.IsTrue(results.AreIdentical);
            Assert.AreEqual(0, results.DifferenceCount);
        }

        /// <summary>
        /// Verifies that only changed lines count as differences.
        /// </summary>
        [TestMethod]
        public void MixedLinesCountOnlyDifferences()
        {
            CodeDiffStudio.Core.ComparisonResults results =
                new CodeDiffStudio.Core.ComparisonResults();

            CodeDiffStudio.Core.LineDifference unchangedLine =
                new CodeDiffStudio.Core.LineDifference();
            unchangedLine.DifferenceType = CodeDiffStudio.Core.DifferenceType.Unchanged;

            CodeDiffStudio.Core.LineDifference addedLine =
                new CodeDiffStudio.Core.LineDifference();
            addedLine.DifferenceType = CodeDiffStudio.Core.DifferenceType.Added;

            CodeDiffStudio.Core.LineDifference removedLine =
                new CodeDiffStudio.Core.LineDifference();
            removedLine.DifferenceType = CodeDiffStudio.Core.DifferenceType.Removed;

            CodeDiffStudio.Core.LineDifference modifiedLine =
                new CodeDiffStudio.Core.LineDifference();
            modifiedLine.DifferenceType = CodeDiffStudio.Core.DifferenceType.Modified;

            results.LineDifferences.Add(unchangedLine);
            results.LineDifferences.Add(addedLine);
            results.LineDifferences.Add(removedLine);
            results.LineDifferences.Add(modifiedLine);

            Assert.IsFalse(results.AreIdentical);
            Assert.AreEqual(3, results.DifferenceCount);
        }

        /// <summary>
        /// Verifies that unchanged lines produce no differences.
        /// </summary>
        [TestMethod]
        public void UnchangedLinesHaveNoDifferences()
        {
            CodeDiffStudio.Core.ComparisonResults results =
                new CodeDiffStudio.Core.ComparisonResults();

            CodeDiffStudio.Core.LineDifference line =
                new CodeDiffStudio.Core.LineDifference();

            line.DifferenceType = CodeDiffStudio.Core.DifferenceType.Unchanged;

            results.LineDifferences.Add(line);
            results.LineDifferences.Add(line);

            Assert.IsTrue(results.AreIdentical);
            Assert.AreEqual(0, results.DifferenceCount);
        }
    }
}
