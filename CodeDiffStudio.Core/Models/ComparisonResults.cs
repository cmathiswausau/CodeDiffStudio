namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Contains the results of a comparison.
    /// </summary>
    public class ComparisonResults
    {
        /// <summary>
        /// Gets a value indicating whether the compared items are identical.
        /// </summary>
        public bool AreIdentical
        {
            get
            {
                foreach (LineDifference lineDifference in this.LineDifferences)
                {
                    if (lineDifference.DifferenceType != DifferenceType.Unchanged)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the number of differences found in the comparison.
        /// </summary>
        public int DifferenceCount
        {
            get
            {
                int differenceCount = 0;

                foreach (LineDifference lineDifference in this.LineDifferences)
                {
                    if (lineDifference.DifferenceType != DifferenceType.Unchanged)
                    {
                        differenceCount++;
                    }
                }

                return differenceCount;
            }
        }

        /// <summary>
        /// Gets or sets the line differences identified in the comparison.
        /// </summary>
        public List<LineDifference> LineDifferences { get; set; } = new List<LineDifference>();

        /// <summary>
        /// Gets or sets the path of the new file.
        /// </summary>
        public string NewFilePath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to the original file.
        /// </summary>
        public string OriginalFilePath { get; set; } = string.Empty;
    }
}
