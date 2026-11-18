namespace CodeDiffStudio.Core
{
    /// <summary>
    /// Compares files and returns their differences.
    /// </summary>
    public class FileComparer
    {
        /// <summary>
        /// Compares lines while preserving matches after insertions and removals.
        /// </summary>
        /// <param name="originalLines">The lines from the original file.</param>
        /// <param name="newLines">The lines from the new file.</param>
        /// <returns>The results of the comparison.</returns>
        public ComparisonResults Compare(string[] originalLines, string[] newLines)
        {
            ComparisonResults results = new ComparisonResults();

            int[,] matchingLineTable =
                this.BuildMatchingLineTable(originalLines, newLines);

            int originalIndex = 0;
            int newIndex = 0;

            while (originalIndex < originalLines.Length ||
                newIndex < newLines.Length)
            {
                LineDifference lineDifference = new LineDifference();

                if (originalIndex >= originalLines.Length)
                {
                    lineDifference.DifferenceType = DifferenceType.Added;
                }
                else if (newIndex >= newLines.Length)
                {
                    lineDifference.DifferenceType = DifferenceType.Removed;
                }
                else if (originalLines[originalIndex] == newLines[newIndex])
                {
                    lineDifference.DifferenceType = DifferenceType.Unchanged;
                }
                else if (matchingLineTable[originalIndex, newIndex] ==
                    matchingLineTable[originalIndex + 1, newIndex + 1])
                {
                    lineDifference.DifferenceType = DifferenceType.Modified;
                }
                else if (matchingLineTable[originalIndex, newIndex + 1] >
                    matchingLineTable[originalIndex + 1, newIndex])
                {
                    lineDifference.DifferenceType = DifferenceType.Added;
                }
                else
                {
                    lineDifference.DifferenceType = DifferenceType.Removed;
                }

                if (lineDifference.DifferenceType != DifferenceType.Added)
                {
                    lineDifference.OriginalLineNumber = originalIndex + 1;
                    lineDifference.OriginalText = originalLines[originalIndex];
                    originalIndex++;
                }

                if (lineDifference.DifferenceType != DifferenceType.Removed)
                {
                    lineDifference.NewLineNumber = newIndex + 1;
                    lineDifference.NewText = newLines[newIndex];
                    newIndex++;
                }

                results.LineDifferences.Add(lineDifference);
            }

            return results;
        }

        /// <summary>
        /// Compares file text while preserving differences in line endings.
        /// </summary>
        /// <param name="originalText">The text from the original file.</param>
        /// <param name="newText">The text from the new file.</param>
        /// <returns>The results of the comparison.</returns>
        public ComparisonResults CompareText(string originalText, string newText)
        {
            string[] originalLines = this.SplitLinesWithEndings(originalText);
            string[] newLines = this.SplitLinesWithEndings(newText);

            return this.Compare(originalLines, newLines);
        }

        /// <summary>
        /// Builds a table containing the number of matching lines remaining from each pair of positions.
        /// </summary>
        /// <param name="originalLines">The lines from the original file.</param>
        /// <param name="newLines">The lines from the new file.</param>
        /// <returns>The table of matching line counts.</returns>
        private int[,] BuildMatchingLineTable(
            string[] originalLines,
            string[] newLines)
        {
            int[,] matchingLineTable =
                new int[originalLines.Length + 1, newLines.Length + 1];

            for (int originalIndex = originalLines.Length - 1;
                originalIndex >= 0;
                originalIndex--)
            {
                for (int newIndex = newLines.Length - 1;
                    newIndex >= 0;
                    newIndex--)
                {
                    if (originalLines[originalIndex] == newLines[newIndex])
                    {
                        matchingLineTable[originalIndex, newIndex] =
                            1 + matchingLineTable[originalIndex + 1, newIndex + 1];
                    }
                    else
                    {
                        matchingLineTable[originalIndex, newIndex] = Math.Max(
                            matchingLineTable[originalIndex + 1, newIndex],
                            matchingLineTable[originalIndex, newIndex + 1]);
                    }
                }
            }

            return matchingLineTable;
        }

        /// <summary>
        /// Splits text into lines, keeping each line ending attached.
        /// </summary>
        /// <param name="text">The text to split into lines.</param>
        /// <returns>The lines with their original line endings.</returns>
        private string[] SplitLinesWithEndings(string text)
        {
            List<string> lines = new List<string>();
            int lineStart = 0;

            for (int index = 0; index < text.Length; index++)
            {
                if (text[index] == '\r' || text[index] == '\n')
                {
                    if (text[index] == '\r' &&
                        index + 1 < text.Length &&
                        text[index + 1] == '\n')
                    {
                        index++;
                    }

                    int lineLength = index - lineStart + 1;
                    string line = text.Substring(lineStart, lineLength);

                    lines.Add(line);
                    lineStart = index + 1;
                }
            }

            if (lineStart < text.Length)
            {
                lines.Add(text.Substring(lineStart));
            }

            return lines.ToArray();
        }
    }
}
