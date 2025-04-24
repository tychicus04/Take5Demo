using Take5Demo.ViewModel;

namespace Take5Demo.Selector
{
    public class StepTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate VisitorManagementTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is StepItem step)
            {
                if (step.StepId == "site-crew")
                {
                    return VisitorManagementTemplate;
                }
            }

            return DefaultTemplate;
        }
    }
}
