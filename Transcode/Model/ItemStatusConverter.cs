using System;
using System.Globalization;
using System.Windows.Data;

namespace Transcode.Model {
	public class ItemStatusConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			if (value == null) { return null; }
			if (value.GetType() != typeof(ItemStatus)) { return null; }
			return value.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotSupportedException();
		}
	}
}
