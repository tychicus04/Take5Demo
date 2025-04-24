namespace Take5Demo.Control;

public partial class CustomEntryItem : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomEntryItem), string.Empty);
    public static readonly BindableProperty PlaceHolderProperty =
        BindableProperty.Create(nameof(PlaceHolder), typeof(string), typeof(CustomEntryItem), string.Empty);

    public event EventHandler<TextChangedEventArgs> TextChanged;
    #endregion 

    #region Properties
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    public string PlaceHolder
    {
        get => (string)GetValue(PlaceHolderProperty);
        set => SetValue(PlaceHolderProperty, value);
    }
    #endregion

    public CustomEntryItem()
	{
		InitializeComponent();
        this.BindingContext = this;
    }

    private void OnEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        TextChanged?.Invoke(this, e);
    }
}