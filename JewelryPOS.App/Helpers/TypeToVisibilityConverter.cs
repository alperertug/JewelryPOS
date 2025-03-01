using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace JewelryPOS.App.Helpers
{
    public class TypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;
            var valueType = value.GetType().Name;
            var expectedType = parameter.ToString();
            return valueType == expectedType ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
