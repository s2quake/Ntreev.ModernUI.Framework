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

using JSSoft.ModernUI.Framework.Dialogs.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JSSoft.ModernUI.Framework.Controls
{
    public class ColorButton : Button
    {
        public static readonly DependencyProperty ValueProperty =
           DependencyProperty.Register(nameof(Value), typeof(Color), typeof(ColorButton),
               new FrameworkPropertyMetadata(Colors.Red, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValuePropertyChangedCallback));

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorButton));

        public Color Value
        {
            get => (Color)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public event RoutedEventHandler ValueChanged
        {
            add { this.AddHandler(ValueChangedEvent, value); }
            remove { this.RemoveHandler(ValueChangedEvent, value); }
        }

        protected override async void OnClick()
        {
            base.OnClick();
            var dialog = new SelectColorViewModel();
            if (await dialog.ShowDialogAsync() == true)
            {
                this.Value = dialog.CurrentColor;
            }
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ColorButton self)
            {
                self.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SelectColorViewModel();
            if (await dialog.ShowDialogAsync() == true)
            {
                this.Value = dialog.CurrentColor;
            }
        }
    }
}
