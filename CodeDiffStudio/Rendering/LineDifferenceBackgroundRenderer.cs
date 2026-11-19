using System.Windows.Media;
using ICSharpCode.AvalonEdit.Rendering;

namespace CodeDiffStudio.Rendering
{
    /// <summary>
    /// Draws backgrounds for lines containing differences.
    /// </summary>
    public class LineDifferenceBackgroundRenderer : IBackgroundRenderer
    {
        /// <summary>
        /// Gets the layer on which line backgrounds are drawn.
        /// </summary>
        public KnownLayer Layer
        {
            get
            {
                return KnownLayer.Background;
            }
        }

        /// <summary>
        /// Gets the background brushes assigned to document line numbers.
        /// </summary>
        public Dictionary<int, Brush> LineBackgrounds { get; } =
            new Dictionary<int, Brush>();

        /// <summary>
        /// Draws difference backgrounds for visible editor lines.
        /// </summary>
        /// <param name="textView">The editor view to draw on.</param>
        /// <param name="drawingContext">The drawing surface.</param>
        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (!textView.VisualLinesValid)
            {
                return;
            }

            foreach (VisualLine visualLine in textView.VisualLines)
            {
                int lineNumber = visualLine.FirstDocumentLine.LineNumber;

                if (this.LineBackgrounds.TryGetValue(lineNumber, out Brush? brush))
                {
                    double top = visualLine.VisualTop - textView.ScrollOffset.Y;

                    System.Windows.Rect backgroundRectangle =
                        new System.Windows.Rect(
                            0,
                            top,
                            textView.ActualWidth,
                            visualLine.Height);

                    drawingContext.DrawRectangle(brush, null, backgroundRectangle);
                }
            }
        }
    }
}
