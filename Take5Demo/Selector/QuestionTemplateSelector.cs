using Take5Demo.Model;

namespace Take5Demo.Selector
{
    public class QuestionTemplateSelector : DataTemplateSelector
    {
        public DataTemplate RadioTemplate { get; set; }
        public DataTemplate ListTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }
        public DataTemplate PickerTemplate { get; set; }
        public DataTemplate SignatureTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is Question question)
            {
                switch (question.ResponseType)
                {
                    case "Radio":
                        return RadioTemplate;
                    case "List":
                        return ListTemplate;
                    case "Text":
                        return TextTemplate;
                    case "Picker":
                        return PickerTemplate;
                    case "Signature":
                        return SignatureTemplate;
                    default:
                        return DefaultTemplate;
                }
            }
            return DefaultTemplate;
        }
    }
}
