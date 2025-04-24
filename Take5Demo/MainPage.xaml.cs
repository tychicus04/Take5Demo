using CommunityToolkit.Maui.Views;
using Take5Demo.Control;
using Take5Demo.Model;
using Take5Demo.ViewModel;
using Microsoft.Maui.Controls.Compatibility;
using StackLayout = Microsoft.Maui.Controls.StackLayout;
using Take5Demo.Helper;
using CommunityToolkit.Maui.Core;
using Grid = Microsoft.Maui.Controls.Grid;

namespace Take5Demo
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        private Dictionary<string, DrawingView> _signaturePads = new Dictionary<string, DrawingView>();

        public MainPage()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            _viewModel.ShowPopupRequest = ShowPopupFromViewModel;
            BindingContext = _viewModel;
        }

        private async Task ShowPopupFromViewModel(string message)
        {
            var popup = new PopupPage(message);
            await this.ShowPopupAsync(popup);
        }
        protected void BindingContextChangedAction(object sender, EventArgs e)
        {
            if (sender is StackLayout stackLayout && stackLayout.BindingContext is Visitor visitor)
            {
                var signaturePad = FindSignaturePad(stackLayout);

                if (signaturePad != null)
                {
                    signaturePad.ClassId = visitor.SignaturePadId;
                    _signaturePads[visitor.SignaturePadId] = signaturePad;
                }
            }
        }
        private DrawingView FindSignaturePad(StackLayout stackLayout)
        {
            foreach (var child in GetAllChildren(stackLayout))
            {
                if (child is DrawingView drawingView)
                {
                    return drawingView;
                }
            }
            return null;
        }

        private IEnumerable<Element> GetAllChildren(Element element)
        {
            var children = new List<Element>();

            if (element is Layout<View> layout)
            {
                foreach (var child in layout.Children)
                {
                    children.Add(child);
                    children.AddRange(GetAllChildren(child));
                }
            }
            else if (element is ContentView contentView && contentView.Content != null)
            {
                children.Add(contentView.Content);
                children.AddRange(GetAllChildren(contentView.Content));
            }
            else if (element is Frame frame && frame.Content != null)
            {
                children.Add(frame.Content);
                children.AddRange(GetAllChildren(frame.Content));
            }
            else if (element is Border border && border.Content != null)
            {
                children.Add(border.Content);
                children.AddRange(GetAllChildren(border.Content));
            }

            return children;
        }

        private void OnQuestionRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.BindingContext is string option)
            {
                var frame = radioButton.FindAncestorOfType<Frame>();
                if (frame?.BindingContext is Question question)
                {
                    _viewModel.HandleRadioButtonCheckedChanged(sender, e, question);
                }
            }
        }

        private void OnQuestionPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is Picker picker)
            {
                var border = picker.FindAncestorOfType<Border>();
                if (border?.BindingContext is Question question)
                {
                    _viewModel.HandlePickerSelectedIndexChanged(sender, e, question);
                }
            }
        }

        private void OnQuestionEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry && entry.BindingContext is Question question)
            {
                _viewModel.HandleEntryTextChanged(sender, e, question);
            }
        }

        private async void DrawingView_DrawingLineCompleted(object sender, DrawingLineCompletedEventArgs e)
        {
            var stream = await (sender as CustomSignaturePadView).GetImageStream(625, 225);
            var parent = (sender as CustomSignaturePadView).Parent;
            var imageSignatureDisplay = parent.FindByName<Image>("imgSignature");
            imageSignatureDisplay.Source = ImageSource.FromStream(() => stream);
        }

        private async void SignatureView_StrokeCompleted(object sender, EventArgs e)
        {
            var gridParent = (Grid)(sender as Button).Parent;
            var parent = (StackLayout)gridParent.Parent;
            var signaturePadView = parent.FindByName<CustomSignaturePadView>("signatureView");
            try
            {
                if (signaturePadView.Lines == null || !signaturePadView.Lines.Any())
                {
                    await DisplayAlert("Error", "Please sign before submitting.", "OK");
                    return;
                }
                var stream = await signaturePadView.GetImageStream(625, 225);
                if (stream != null)
                {
                    stream.Position = 0;
                    var question = signaturePadView.BindingContext as Question;

                    if (question != null)
                    {
                        var binaryReader = new BinaryReader(stream);
                        binaryReader.BaseStream.Position = 0;
                        byte[] signatures = binaryReader.ReadBytes((int)stream.Length);
                        var base64Str = StringHelper.ToBase64(signatures, true);

                        _viewModel.HandleSignatureCompleted(signaturePadView, e, question, base64Str);
                    }
                    else
            {
                await DisplayAlert("Error", "Question information is missing.", "OK");
            }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private void SignatureView_Cleared(object sender, EventArgs e)
        {
            try
            {
                var gridParent = (Grid)(sender as Button).Parent;
                var parent = (StackLayout)gridParent.Parent;
                var signaturePadView = parent.FindByName<CustomSignaturePadView>("signatureView");
                var question = signaturePadView.BindingContext as Question;
                if (question != null)
                {
                    _viewModel.HandleSignatureCleared(signaturePadView, e, question);
                }
                signaturePadView.Clear();
                var imageSignatureDisplay = parent.FindByName<Image>("imgSignature");
                imageSignatureDisplay.Source = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}