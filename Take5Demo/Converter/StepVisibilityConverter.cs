using System.Globalization;
namespace Take5Demo.Converter
{
    public class StepVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int currentStep && parameter is string paramStep)
            {
                if (int.TryParse(paramStep, out int stepNumber))
                {
                    return currentStep == stepNumber;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
