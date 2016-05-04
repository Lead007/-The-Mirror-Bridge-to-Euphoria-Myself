using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media;

namespace JLQ_MBE_BattleSimulation
{
    [ValueConversion(typeof(Brush), typeof(string))]
    class ConverterContentToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "准备阶段":
                    return Brushes.Red;
                case "行动阶段":
                    return Brushes.DarkOrange;
                case "结束阶段":
                    return Brushes.CornflowerBlue;
                default:
                    return Brushes.Blue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((string)value)
            {
                case "准备阶段":
                    return Brushes.Red;
                case "行动阶段":
                    return Brushes.DarkOrange;
                case "结束阶段":
                    return Brushes.CornflowerBlue;
                default:
                    return Brushes.Blue;
            }
        }
    }

    [ValueConversion(typeof(Brush), typeof(string))]
    class ConverterHasMovedToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "还未移动" ? Brushes.Red : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }

    [ValueConversion(typeof(Brush), typeof(string))]
    class ConverterHasAttackedToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value == "还未攻击" ? Brushes.Red : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
}
