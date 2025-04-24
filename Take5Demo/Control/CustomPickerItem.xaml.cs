using System.Collections;
using System.Windows.Input;
namespace Take5Demo.Control;

public partial class CustomPickerItem : ContentView
{
    public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(string), typeof(CustomPickerItem), string.Empty);

    public static readonly BindableProperty PlaceHolderTitleProperty =
            BindableProperty.Create(nameof(PlaceHolderTitle), typeof(string), typeof(CustomPickerItem), "Select an option");

    public static readonly BindableProperty SelectedItemProperty =
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(CustomPickerItem), null,
                BindingMode.TwoWay);

    public static readonly BindableProperty ItemsProperty =
            BindableProperty.Create(nameof(Items), typeof(IList), typeof(CustomPickerItem), null);

    public static readonly BindableProperty SelectedIndexProperty =
           BindableProperty.Create(nameof(SelectedIndex), typeof(int), typeof(CustomPickerItem), -1,
               BindingMode.TwoWay, propertyChanged: OnSelectedIndexChanged);

    public static readonly BindableProperty SelectedItemChangedCommandProperty =
        BindableProperty.Create(nameof(SelectedItemChangedCommand), typeof(ICommand), typeof(CustomPickerItem), null);

    public IList Items
    {
        get => (IList)GetValue(ItemsProperty);
        set => SetValue(ItemsProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string PlaceHolderTitle
    {
        get => (string)GetValue(PlaceHolderTitleProperty);
        set => SetValue(PlaceHolderTitleProperty, value);
    }
    public object SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    public ICommand SelectedItemChangedCommand
    {
        get => (ICommand)GetValue(SelectedItemChangedCommandProperty);
        set => SetValue(SelectedItemChangedCommandProperty, value);
    }

    public CustomPickerItem()
	{
		InitializeComponent();
        this.BindingContext = this;
    }

    private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomPickerItem)bindable;

        if (control.Items != null && newValue != null)
        {
            var index = control.Items.IndexOf(newValue);
            if (index != control.SelectedIndex)
            {
                control.SelectedIndex = index;
            }
        }
    }

    private static void OnSelectedIndexChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (CustomPickerItem)bindable;
        var index = (int)newValue;

        if (control.Items != null && index >= 0 && index < control.Items.Count)
        {
            var item = control.Items[index];
            if (!object.Equals(item, control.SelectedItem))
            {
                control.SelectedItem = item;
            }
        }
    }

    private void Picker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var picker = (Picker)sender;

        if (picker.SelectedIndex != -1)
        {
            SelectedIndex = picker.SelectedIndex;
            SelectedItem = Items?[picker.SelectedIndex];

            // Execute command if assigned
            if (SelectedItemChangedCommand != null && SelectedItemChangedCommand.CanExecute(SelectedItem))
            {
                SelectedItemChangedCommand.Execute(SelectedItem);
            }
        }
    }
}