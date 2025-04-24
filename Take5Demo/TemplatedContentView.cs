namespace Take5Demo;

public class TemplatedContentView : ContentView
{
    public static readonly BindableProperty ItemTemplateProperty =
        BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(TemplatedContentView), null);

    public static readonly BindableProperty ItemTemplateSelectorProperty =
        BindableProperty.Create(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(TemplatedContentView), null);

    public static readonly BindableProperty ItemProperty =
        BindableProperty.Create(nameof(Item), typeof(object), typeof(TemplatedContentView), null, propertyChanged: OnItemChanged);

    public DataTemplate ItemTemplate
    {
        get => (DataTemplate)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public DataTemplateSelector ItemTemplateSelector
    {
        get => (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty);
        set => SetValue(ItemTemplateSelectorProperty, value);
    }

    public object Item
    {
        get => GetValue(ItemProperty);
        set => SetValue(ItemProperty, value);
    }

    private static void OnItemChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = (TemplatedContentView)bindable;
        control.UpdateContent();
    }

    private void UpdateContent()
    {
        if (Item == null)
        {
            Content = null;
            return;
        }

        DataTemplate template = null;

        if (ItemTemplateSelector != null)
            template = ItemTemplateSelector.SelectTemplate(Item, this);

        if (template == null)
            template = ItemTemplate;

        if (template == null)
            return;

        var content = template.CreateContent();
        if (content is View view)
        {
            view.BindingContext = Item;
            Content = view;
        }
    }
}