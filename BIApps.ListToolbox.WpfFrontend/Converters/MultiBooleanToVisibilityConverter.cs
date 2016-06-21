using System;
using System.Linq;
using System.Windows.Data;

namespace BIApps.ListToolbox.WpfFrontend.Converters {
    public class MultiBooleanToVisibilityConverter : IMultiValueConverter {
        public object Convert(object[] values,
                                Type targetType,
                                object parameter,
                                System.Globalization.CultureInfo culture) {

            var visible = values.OfType<bool>().Aggregate(false, (current, value) => current || value);

            return visible
                ? System.Windows.Visibility.Visible
                : System.Windows.Visibility.Collapsed;
        }

        public object[] ConvertBack(object value,
                                    Type[] targetTypes,
                                    object parameter,
                                    System.Globalization.CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
