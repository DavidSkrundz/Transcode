using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Transcode.Model {
	public class BoolVisibilityConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) { return null; }
			if (value.GetType() != typeof(bool)) { return null; }
			if (System.Convert.ToBoolean(parameter, CultureInfo.InvariantCulture)) {
				return (bool)value ? Visibility.Visible : Visibility.Hidden;
			} else {
				return (bool)value ? Visibility.Hidden: Visibility.Visible;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
