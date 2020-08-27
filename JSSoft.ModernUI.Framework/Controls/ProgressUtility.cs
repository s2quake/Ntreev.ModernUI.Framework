// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using System.Windows;

namespace JSSoft.ModernUI.Framework.Controls
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
