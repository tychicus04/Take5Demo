using System.Windows.Input;

namespace Take5Demo.Control;

public partial class CustomSwitchItem : ContentView
{
    #region Bindable Properties
    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(CustomSwitchItem), false,
            BindingMode.TwoWay);

    public static readonly BindableProperty QuestionTextProperty =
        BindableProperty.Create(nameof(QuestionText), typeof(string), typeof(CustomSwitchItem), string.Empty);

    public static readonly BindableProperty ToggledCommandProperty =
        BindableProperty.Create(nameof(ToggledCommand), typeof(ICommand), typeof(CustomSwitchItem), null);
    #endregion

    #region Properties
    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public string QuestionText
    {
        get => (string)GetValue(QuestionTextProperty);
        set => SetValue(QuestionTextProperty, value);
    }

    public ICommand ToggledCommand
    {
        get => (ICommand)GetValue(ToggledCommandProperty);
        set => SetValue(ToggledCommandProperty, value);
    }
    #endregion

    public event EventHandler<ToggledEventArgs> Toggled;

    public CustomSwitchItem()
	{
		InitializeComponent();
        this.BindingContext = this;
    }

    private void OnSwitchToggled(object sender, ToggledEventArgs e)
    {
        Toggled?.Invoke(this, e);
        ToggledCommand?.Execute(e.Value);
    }
}