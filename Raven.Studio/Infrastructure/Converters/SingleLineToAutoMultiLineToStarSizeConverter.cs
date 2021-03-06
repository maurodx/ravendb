﻿using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Raven.Studio.Infrastructure.Converters
{
    public class SingleLineToAutoMultiLineToStarSizeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var stringValue = value as string;
            
            if (string.IsNullOrEmpty(stringValue))
            {
                return GridLength.Auto;
            }

            return stringValue.Contains(Environment.NewLine) ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
