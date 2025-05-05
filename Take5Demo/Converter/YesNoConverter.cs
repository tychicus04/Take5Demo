using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Take5Demo.Converter
{
    public class YesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Chuyển đổi từ Answer (string) sang IsChecked (bool)
            if (value == null) return false;

            // parameter là câu trả lời mong đợi (ví dụ: "Yes" hoặc "No")
            string expectedAnswer = parameter as string;
            if (string.IsNullOrEmpty(expectedAnswer)) return false;

            // Kiểm tra xem Answer hiện tại có khớp với câu trả lời mong đợi không
            string currentAnswer = value as string;
            if (string.IsNullOrEmpty(currentAnswer)) return false;

            return string.Equals(currentAnswer, expectedAnswer, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Chuyển đổi từ IsChecked (bool) sang Answer (string)
            if (!(value is bool isChecked) || !isChecked) return null;

            // parameter là câu trả lời mong đợi (ví dụ: "Yes" hoặc "No")
            string answer = parameter as string;
            if (string.IsNullOrEmpty(answer)) return null;

            // Khi được chọn, trả về giá trị tham số (ví dụ: "Yes" hoặc "No")
            return answer;
        }
    }
}
