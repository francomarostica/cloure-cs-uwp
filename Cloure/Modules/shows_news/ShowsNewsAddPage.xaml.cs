using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// La plantilla de elemento Página en blanco está documentada en https://go.microsoft.com/fwlink/?LinkId=234238

namespace Cloure.Modules.shows_news
{
    /// <summary>
    /// Una página vacía que se puede usar de forma independiente o a la que se puede navegar dentro de un objeto Frame.
    /// </summary>
    public sealed partial class ShowsNewsAddPage : Page
    {
        private ShowArticle showArticle;

        public ShowsNewsAddPage()
        {
            this.InitializeComponent();

            txtFontSize.SelectedValue = 12;

            showArticle = new ShowArticle();
            showArticle.Id = 0;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter != null)
            {
                if (e.Parameter.GetType() == typeof(int))
                {
                    showArticle.Id = (int)e.Parameter;
                    Load_Data();
                }
            }
        }

        private async void Load_Data()
        {
            showArticle = await ShowsNews.Get(showArticle.Id);
            editor.Document.SetText(TextSetOptions.FormatRtf, showArticle.Contenido);
        }

        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                try
                {
                    Windows.Storage.Streams.IRandomAccessStream randAccStream =
                await file.OpenAsync(Windows.Storage.FileAccessMode.Read);

                    // Load the file into the Document property of the RichEditBox.
                    editor.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, randAccStream);
                }
                catch (Exception)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = "File open error",
                        Content = "Sorry, I couldn't open the file.",
                        PrimaryButtonText = "Ok"
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });

            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "New Document";

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until we
                // finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                Windows.Storage.Streams.IRandomAccessStream randAccStream =
                    await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);

                editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);

                // Let Windows know that we're finished changing the file so the
                // other app can update the remote version of the file.
                Windows.Storage.Provider.FileUpdateStatus status = await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status != Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
        }

        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Bold = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.UI.Text.ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Italic = Windows.UI.Text.FormatEffect.Toggle;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == Windows.UI.Text.UnderlineType.None)
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.Single;
                }
                else
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void SetFontSize(int size)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                charFormatting.Size = size;
                selectedText.CharacterFormat = charFormatting;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            CloureManager.GoBack();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private async void Save()
        {
            string document_content = "";

            editor.Document.GetText(TextGetOptions.FormatRtf, out document_content);

            showArticle.Contenido = document_content;
            showArticle.Titulo = "Test";

            bool result = await ShowsNews.save(showArticle);
            if (result)
            {
                CloureManager.GoBack();
            }
        }

        private void TxtFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFontSize((int)txtFontSize.SelectedValue);
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void InsertImageButtton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker open = new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".jpg");
            open.FileTypeFilter.Add(".png");
            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage image = new BitmapImage();
                    await image.SetSourceAsync(fileStream);
                    editor.Document.Selection.InsertImage(image.PixelWidth, image.PixelHeight, 0, VerticalCharacterAlignment.Baseline, "img", fileStream);
                }
            }
        }

        private void InsertLinkButtton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void InsertVideoLinkButtton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AlignLeftButtton_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat charFormatting = selectedText.ParagraphFormat;
                selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Left;
            }
        }

        private void AlignCenterButtton_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat charFormatting = selectedText.ParagraphFormat;
                selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Center;
            }
        }

        private void AlignRightButtton_Click(object sender, RoutedEventArgs e)
        {
            ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                ITextParagraphFormat charFormatting = selectedText.ParagraphFormat;
                selectedText.ParagraphFormat.Alignment = ParagraphAlignment.Right;
            }
        }
    }
}
