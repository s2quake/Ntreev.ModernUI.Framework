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
using System.Windows.Controls;

namespace JSSoft.ModernUI.Framework.Controls
{
    public class ErrorBlinker : UserControl
    {
        private static readonly DependencyPropertyKey HasErrorPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasError), typeof(bool), typeof(ErrorBlinker),
                new PropertyMetadata(false));
        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        public static readonly DependencyProperty ErrorContentProperty =
            DependencyProperty.Register(nameof(ErrorContent), typeof(object), typeof(ErrorBlinker),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsArrange, ErrorContentPropertyPropertyChangedCallback));

        public ErrorBlinker()
        {

        }

        public bool HasError => (bool)this.GetValue(HasErrorProperty);

        public object ErrorContent
        {
            get => (object)this.GetValue(ErrorContentProperty);
            set => this.SetValue(ErrorContentProperty, value);
        }

        private static void ErrorContentPropertyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is string text)
            {
                d.SetValue(HasErrorPropertyKey, string.IsNullOrEmpty(text) == false);
            }
            else
            {
                d.SetValue(HasErrorPropertyKey, e.NewValue != null);
            }
        }
    }
}
