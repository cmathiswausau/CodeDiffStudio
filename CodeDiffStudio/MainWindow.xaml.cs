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
        /// Compares the loaded file text.
        /// </summary>
        private readonly CodeDiffStudio.Core.IFileComparer fileComparer;

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
        /// Reads the selected text files.
        /// </summary>
        private readonly CodeDiffStudio.Core.ITextFileReader textFileReader;

        /// <summary>
        /// Indicates whether a file-opening or refresh operation is running.
        /// </summary>
        private bool isFileOperationRunning;

        /// <summary>
        /// Contains the successfully loaded left file.
        /// </summary>
        private CodeDiffStudio.Core.FileSnapshot? leftSnapshot;

        /// <summary>
        /// Contains the successfully loaded right file.
        /// </summary>
        private CodeDiffStudio.Core.FileSnapshot? rightSnapshot;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
            : this(
                new CodeDiffStudio.Core.TextFileReader(),
                new CodeDiffStudio.Core.FileComparer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class with the specified
        /// text file reader.
        /// </summary>
        /// <param name="textFileReader">The reader used to load text files.</param>
        public MainWindow(CodeDiffStudio.Core.ITextFileReader textFileReader)
            : this(textFileReader, new CodeDiffStudio.Core.FileComparer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class with the specified
        /// file reader and comparer.
        /// </summary>
        /// <param name="textFileReader">The reader used to load text files.</param>
        /// <param name="fileComparer">The service used to compare text.</param>
        public MainWindow(
            CodeDiffStudio.Core.ITextFileReader textFileReader,
            CodeDiffStudio.Core.IFileComparer fileComparer)
        {
            ArgumentNullException.ThrowIfNull(textFileReader);
            ArgumentNullException.ThrowIfNull(fileComparer);

            this.textFileReader = textFileReader;
            this.fileComparer = fileComparer;
            this.InitializeComponent();

            this.LeftFileContents.TextArea.TextView.BackgroundRenderers.Add(
                this.leftBackgroundRenderer);

            this.RightFileContents.TextArea.TextView.BackgroundRenderers.Add(
                this.rightBackgroundRenderer);

            this.UpdateFileControls();
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
            if (this.isFileOperationRunning)
            {
                return;
            }

            var picker = new Microsoft.Win32.OpenFileDialog
            {
                Title = pickerTitle,
                CheckFileExists = true,
            };

            if (picker.ShowDialog(this) != true)
            {
                return;
            }

            this.isFileOperationRunning = true;
            this.UpdateFileControls();

            try
            {
                CodeDiffStudio.Core.FileSnapshot snapshot =
                    await this.textFileReader.ReadAsync(picker.FileName);

                fileContents.Text = snapshot.Text;
                fileName.Text = System.IO.Path.GetFileName(snapshot.FilePath);

                switch (paneSelection)
                {
                    case PaneSelection.Left:
                        this.leftSnapshot = snapshot;
                        break;

                    case PaneSelection.Right:
                        this.rightSnapshot = snapshot;
                        break;
                }

                this.UpdateComparisonStatus();
            }
            catch (System.Exception ex) when (
                ex is System.IO.IOException or System.UnauthorizedAccessException)
            {
                this.ShowFileError(
                    picker.FileName,
                    "open the file",
                    ex is System.UnauthorizedAccessException ? "Access was denied. Choose another file." : "Try opening the file again or choose another file.");
            }
            finally
            {
                this.isFileOperationRunning = false;
                this.UpdateFileControls();
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
            if (this.isFileOperationRunning ||
                this.leftSnapshot is not CodeDiffStudio.Core.FileSnapshot leftFile ||
                this.rightSnapshot is not CodeDiffStudio.Core.FileSnapshot rightFile)
            {
                return;
            }

            this.isFileOperationRunning = true;
            this.UpdateFileControls();

            string pathBeingRead = leftFile.FilePath;

            try
            {
                CodeDiffStudio.Core.FileSnapshot newLeftSnapshot =
                    await this.textFileReader.ReadAsync(leftFile.FilePath);

                pathBeingRead = rightFile.FilePath;

                CodeDiffStudio.Core.FileSnapshot newRightSnapshot =
                    await this.textFileReader.ReadAsync(rightFile.FilePath);

                this.leftSnapshot = newLeftSnapshot;
                this.rightSnapshot = newRightSnapshot;
                this.LeftFileContents.Text = newLeftSnapshot.Text;
                this.RightFileContents.Text = newRightSnapshot.Text;

                this.UpdateComparisonStatus();
            }
            catch (System.Exception ex) when (
                ex is System.IO.IOException or System.UnauthorizedAccessException)
            {
                string recoveryInstruction = ex is System.UnauthorizedAccessException
                    ? "Access was denied. Restore access and press F5, or choose a replacement file."
                    : "Press F5 to retry or choose a replacement file.";

                this.ShowFileError(
                    pathBeingRead,
                    "reload the file",
                    recoveryInstruction);
            }
            finally
            {
                this.isFileOperationRunning = false;
                this.UpdateFileControls();
            }
        }

        /// <summary>
        /// Displays a file error while preserving the previous comparison.
        /// </summary>
        /// <param name="filePath">The full path of the file that failed.</param>
        /// <param name="operation">The operation that failed.</param>
        /// <param name="recoveryInstruction">Instructions for recovering from the error.</param>
        private void ShowFileError(
            string filePath,
            string operation,
            string recoveryInstruction)
        {
            this.UpdateComparisonStatus();

            string previousResult = string.Empty;

            if (this.leftSnapshot is not null && this.rightSnapshot is not null)
            {
                previousResult =
                    $"Showing previous comparison. {this.ComparisonStatus.Text}\n";
            }

            this.ComparisonStatus.Text =
                $"{previousResult}Couldn't {operation}:\n{filePath}\n{recoveryInstruction}";

            this.ComparisonStatus.CaretIndex = 0;
            this.ComparisonStatus.ScrollToHome();
            this.ComparisonStatus.Focus();
        }

        /// <summary>
        /// Updates the comparison status after both files have loaded.
        /// </summary>
        private void UpdateComparisonStatus()
        {
            if (this.leftSnapshot == null || this.rightSnapshot == null)
            {
                this.ComparisonStatus.Text = "Open two files to compare.";
                return;
            }

            CodeDiffStudio.Core.ComparisonResults results =
                this.fileComparer.CompareText(
                    this.leftSnapshot.Text,
                    this.rightSnapshot.Text);

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

        /// <summary>
        /// Updates file controls to reflect the current operation state.
        /// </summary>
        private void UpdateFileControls()
        {
            this.OpenLeftButton.IsEnabled = !this.isFileOperationRunning;
            this.OpenRightButton.IsEnabled = !this.isFileOperationRunning;

            this.RefreshButton.IsEnabled =
                !this.isFileOperationRunning &&
                this.leftSnapshot is not null &&
                this.rightSnapshot is not null;
        }
    }
}
