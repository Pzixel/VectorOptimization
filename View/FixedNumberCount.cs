using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace View
{
    [ValueConversion(typeof(double),typeof(string))]
    public class FixedNumberCountConverter :IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = ((double) value).ToString(culture);
            return input.Substring(0, Math.Min(input.Length, 7));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = value.ToString();
            double result;
            if (double.TryParse(input, NumberStyles.Any, culture, out result))
                return result;
            return value;
        }
    }

 //   [ValueConversion(typeof(ListViewItem), typeof(string))]
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem) value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}