using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CodeDiffStudio.Rendering;

namespace CodeDiffStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Draws difference backgrounds in the left editor.
        /// </summary>
        private readonly LineDifferenceBackgroundRenderer leftBackgroundRenderer =
            new LineDifferenceBackgroundRenderer();

        /// <summary>
        /// Draws difference backgrounds in the right editor.
        /// </summary>
        private readonly LineDifferenceBackgroundRenderer rightBackgroundRenderer =
            new LineDifferenceBackgroundRenderer();

        /// <summary>
        /// The path to the left file.
        /// </summary>
        private string? leftFilePath;

        /// <summary>
        /// Text for the left side, or null if unavailable.
        /// </summary>
        private string? leftText;

        /// <summary>
        /// Path to the file on the right side of the comparison.
        /// </summary>
        /// <remarks>Null when no right-side file is selected.</remarks>
        private string? rightFilePath;

        /// <summary>
        /// The text displayed on the right side.
        /// </summary>
        private string? rightText;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            this.LeftFileContents.TextArea.TextView.BackgroundRenderers.Add(this.leftBackgroundRenderer);

            this.RightFileContents.TextArea.TextView.BackgroundRenderers.Add(this.rightBackgroundRenderer);
        }

        /// <summary>
        /// Handles the F5 refresh shortcut.
        /// </summary>
        /// <param name="sender">The window that raised the event.</param>
        /// <param name="e">The keyboard event data.</param>
        private async void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 && this.RefreshButton.IsEnabled)
            {
                e.Handled = true;
                await this.RefreshFilesAsync();
            }
        }

        /// <summary>
        /// Opens a file picker and loads the selected file into a comparison pane.
        /// </summary>
        /// <param name="paneSelection">The pane that receives the file.</param>
        /// <param name="openButton">The button that opens the picker for the pane.</param>
        /// <param name="fileName">The label that displays the selected file name.</param>
        /// <param name="fileContents">The editor that displays the file contents.</param>
        /// <param name="pickerTitle">The title of the file picker.</param>
        /// <returns>A task that completes when the file-opening operation finishes.</returns>
        private async Task OpenFileAsync(
            PaneSelection paneSelection,
            Button openButton,
            TextBlock fileName,
            ICSharpCode.AvalonEdit.TextEditor fileContents,
            string pickerTitle)
        {
            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = pickerTitle,
                CheckFileExists = true,
            };

            if (picker.ShowDialog(this) != true)
            {
                return;
            }

            openButton.IsEnabled = false;

            try
            {
                var contents = await System.IO.File.ReadAllTextAsync(picker.FileName);
                fileContents.Text = contents;
                fileName.Text = System.IO.Path.GetFileName(picker.FileName);

                switch (paneSelection)
                {
                    case PaneSelection.Left:
                        this.leftText = contents;

                        // In case PaneSelection.Left:
                        this.leftFilePath = picker.FileName;
                        break;

                    case PaneSelection.Right:
                        this.rightText = contents;

                        // In case PaneSelection.Right:
                        this.rightFilePath = picker.FileName;
                        break;
                }

                this.UpdateComparisonStatus();
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show(this, "Couldn't read the file. Try again or choose another file.");
            }
            catch (System.UnauthorizedAccessException)
            {
                MessageBox.Show(this, "Access to the file was denied. Choose another file.");
            }
            finally
            {
                openButton.IsEnabled = true;
                this.RefreshButton.IsEnabled =
                this.leftFilePath is not null &&
                this.rightFilePath is not null &&
                this.OpenLeftButton.IsEnabled &&
                this.OpenRightButton.IsEnabled;
            }
        }

        /// <summary>
        /// Opens the file picker for the left pane.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">The click event data.</param>
        private async void OpenLeft_Click(object sender, RoutedEventArgs e)
        {
            await this.OpenFileAsync(
                PaneSelection.Left,
                this.OpenLeftButton,
                this.LeftFileName,
                this.LeftFileContents,
                "Open left file");
        }

        /// <summary>
        /// Opens the right-side file.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">The event data.</param>
        private async void OpenRight_Click(object sender, RoutedEventArgs e)
        {
            await this.OpenFileAsync(
                PaneSelection.Right,
                this.OpenRightButton,
                this.RightFileName,
                this.RightFileContents,
                "Open right file");
        }

        /// <summary>
        /// Refreshes the selected file pair when the button is clicked.
        /// </summary>
        /// <param name="sender">The button that raised the event.</param>
        /// <param name="e">The click event data.</param>
        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await this.RefreshFilesAsync();
        }

        /// <summary>
        /// Reloads both selected files and updates the comparison after both reads succeed.
        /// </summary>
        /// <returns>A task that completes when the refresh finishes.</returns>
        private async Task RefreshFilesAsync()
        {
            if (this.leftFilePath is not string leftPath ||
                this.rightFilePath is not string rightPath)
            {
                return;
            }

            this.RefreshButton.IsEnabled = false;
            this.OpenLeftButton.IsEnabled = false;
            this.OpenRightButton.IsEnabled = false;

            string pathBeingRead = leftPath;

            try
            {
                string newLeftText = await System.IO.File.ReadAllTextAsync(leftPath);

                pathBeingRead = rightPath;
                string newRightText = await System.IO.File.ReadAllTextAsync(rightPath);

                this.leftText = newLeftText;
                this.rightText = newRightText;
                this.LeftFileContents.Text = newLeftText;
                this.RightFileContents.Text = newRightText;
                this.UpdateComparisonStatus();
            }
            catch (System.Exception ex) when (
                ex is System.IO.IOException or System.UnauthorizedAccessException)
            {
                this.UpdateComparisonStatus();
                string previousResult = this.ComparisonStatus.Text;
                this.ComparisonStatus.Text =
                    $"Showing previous comparison. {previousResult} " +
                    $"Couldn't reload {pathBeingRead}. Retry or choose a replacement file.";
            }
            finally
            {
                this.OpenLeftButton.IsEnabled = true;
                this.OpenRightButton.IsEnabled = true;
                this.RefreshButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// Updates the comparison status after both files have loaded.
        /// </summary>
        private void UpdateComparisonStatus()
        {
            if (this.leftText == null || this.rightText == null)
            {
                return;
            }

            CodeDiffStudio.Core.FileComparer comparer =
                new CodeDiffStudio.Core.FileComparer();

            CodeDiffStudio.Core.ComparisonResults results =
                comparer.CompareText(this.leftText, this.rightText);

            this.UpdateDifferenceBackgrounds(results);

            if (results.AreIdentical)
            {
                this.ComparisonStatus.Text = "Files are identical.";
            }
            else
            {
                this.ComparisonStatus.Text =
                    $"Files differ. Differences: {results.DifferenceCount}.";
            }
        }

        /// <summary>
        /// Updates line backgrounds using the comparison results.
        /// </summary>
        /// <param name="results">The differences to highlight.</param>
        private void UpdateDifferenceBackgrounds(
            CodeDiffStudio.Core.ComparisonResults results)
        {
            this.leftBackgroundRenderer.LineBackgrounds.Clear();
            this.rightBackgroundRenderer.LineBackgrounds.Clear();

            foreach (CodeDiffStudio.Core.LineDifference line in results.LineDifferences)
            {
                switch (line.DifferenceType)
                {
                    case CodeDiffStudio.Core.DifferenceType.Added:
                        this.rightBackgroundRenderer.LineBackgrounds[line.NewLineNumber] =
                            System.Windows.Media.Brushes.LightGreen;
                        break;

                    case CodeDiffStudio.Core.DifferenceType.Removed:
                        this.leftBackgroundRenderer.LineBackgrounds[line.OriginalLineNumber] =
                            System.Windows.Media.Brushes.LightPink;
                        break;

                    case CodeDiffStudio.Core.DifferenceType.Modified:
                        this.leftBackgroundRenderer.LineBackgrounds[line.OriginalLineNumber] =
                            System.Windows.Media.Brushes.Gold;

                        this.rightBackgroundRenderer.LineBackgrounds[line.NewLineNumber] =
                            System.Windows.Media.Brushes.Gold;
                        break;
                }
            }

            this.LeftFileContents.TextArea.TextView.InvalidateLayer(
                ICSharpCode.AvalonEdit.Rendering.KnownLayer.Background);

            this.RightFileContents.TextArea.TextView.InvalidateLayer(
                ICSharpCode.AvalonEdit.Rendering.KnownLayer.Background);
        }
    }
}
