using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GameController
{
    public class PointConverter : IMultiValueConverter
    {
        // 从源（ViewModel的两个属性）到目标（TextBox.Text）的转换
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] != null && values[1] != null)
            {
                // 假设两个属性都是数字类型，可根据实际情况调整
                return $"({values[0]}, {values[1]})";
            }
            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string str)
            {
                var paras = str.Split(',');
                if(paras.Length >= 2)
                {
                    var trueColor = (Color)ColorConverter.ConvertFromString(paras[0]);
                    var falseColor = (Color)ColorConverter.ConvertFromString(paras[1]);
                    return boolValue ? new SolidColorBrush(trueColor) : new SolidColorBrush(falseColor);
                }
            }
            return Brushes.Transparent; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 通常不需要反向转换，如果不需要双向绑定可抛出异常
            throw new NotSupportedException();
        }
    }

    public class ValueToPointValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int i && parameter is string str)
            {
                var parts = str.Split(',');
                if (parts.Length >= 2 && double.TryParse(parts[0], out double r) && double.TryParse(parts[1], out double s))
                    return (i + 32768) / 65535.0 * r - s/2;
            }
            return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueToPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int i)
            {
                return i/ 65535.0;
            }
            return 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
