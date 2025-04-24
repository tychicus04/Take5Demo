using CommunityToolkit.Maui.Views;

namespace Take5Demo.Control;

public partial class PopupPage : Popup
{
    public PopupPage()
    {
        InitializeComponent();
    }

    public PopupPage(string errorMessage)
    {
        InitializeComponent();
        ErrorMessageLabel.Text = errorMessage;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        Close();
    }
}