using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Ntreev.ModernUI.Framework.Controls
{
    public static class ProgressUtility
    {
        private const string IsProgressing = nameof(IsProgressing);
        private const string Message = nameof(Message);

        public static readonly DependencyProperty IsProgressingProperty =
           DependencyProperty.RegisterAttached(nameof(IsProgressing), typeof(bool), typeof(ProgressUtility),
               new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty MessageProperty =
           DependencyProperty.RegisterAttached(nameof(Message), typeof(string), typeof(ProgressUtility),
               new FrameworkPropertyMetadata(string.Empty, MessagePropertyChangedCallback, MessagePropertyCoerceValueCallback));

        public static bool GetIsProgressing(DependencyObject d)
        {
            return (bool)d.GetValue(IsProgressingProperty);
        }

        public static void SetIsProgressing(DependencyObject d, bool value)
        {
            d.SetValue(IsProgressingProperty, value);
        }

        public static string GetMessage(DependencyObject d)
        {
            return (string)d.GetValue(MessageProperty);
        }

        public static void SetMessage(DependencyObject d, string value)
        {
            d.SetValue(MessageProperty, value);
        }

        private static void MessagePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object MessagePropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            return $"{baseValue}";
        }
    }
}
