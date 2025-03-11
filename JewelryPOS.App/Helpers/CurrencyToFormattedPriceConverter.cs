using JewelryPOS.App.Enums;
using System.Globalization;
using System.Windows.Data;

namespace JewelryPOS.App.Helpers
{
    public class CurrencyToFormattedPriceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 2 || values[0] == null || values[1] == null)
                return "Fiyat Yok";

            if (!(values[0] is decimal price) || !(values[1] is Currency currency))
                return "Fiyat Yok";

            string symbol = GetCurrencySymbol(currency);
            return $"{symbol}{price:F2}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private string GetCurrencySymbol(Currency currency)
        {
            return currency switch
            {
                Currency.TRY => "₺",
                Currency.USD => "$",
                Currency.EUR => "€",
                _ => ""
            };
        }
    }
}
