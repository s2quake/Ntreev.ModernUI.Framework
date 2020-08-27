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

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace JSSoft.ModernUI.Framework.Controls
{
    public partial class ColorPicker : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    ValuePropertyChangedCallback));

        private static readonly DependencyPropertyKey HasErrorPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasError), typeof(bool), typeof(ColorPicker), new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasErrorProperty = HasErrorPropertyKey.DependencyProperty;

        private static readonly DependencyPropertyKey ErrorContentPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(ErrorContent), typeof(object), typeof(ColorPicker), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ErrorContentProperty = ErrorContentPropertyKey.DependencyProperty;

        public static readonly RoutedEvent ValueChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanged), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ColorPicker));

        public static readonly RoutedEvent ErrorEvent =
            EventManager.RegisterRoutedEvent(nameof(Error), RoutingStrategy.Bubble, typeof(EventHandler<ErrorEventArgs>), typeof(ColorPicker));

        private Button button;

        public ColorPicker()
        {
            InitializeComponent();
        }

        public Color Value
        {
            get => (Color)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public bool HasError => (bool)this.GetValue(HasErrorProperty);

        public object ErrorContent => (bool)this.GetValue(ErrorContentProperty);

        public event RoutedEventHandler ValueChanged
        {
            add { this.AddHandler(ValueChangedEvent, value); }
            remove { this.AddHandler(ValueChangedEvent, value); }
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add { this.AddHandler(ErrorEvent, value); }
            remove { this.AddHandler(ErrorEvent, value); }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Background is SolidColorBrush brush)
            {
                this.Value = brush.Color;
                if (this.button != null)
                    this.button.Tag = null;
                this.button = button;
                if (this.button != null)
                    this.button.Tag = true;
            }
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                element.RaiseEvent(new RoutedEventArgs(ValueChangedEvent));
            }
        }

        private void UserControl_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                this.SetValue(HasErrorPropertyKey, true);
                this.SetValue(ErrorContentPropertyKey, e.Error.ErrorContent);
                this.RaiseEvent(new ErrorEventArgs(ErrorEvent, e.Error.ErrorContent));
            }
            else
            {
                this.SetValue(HasErrorPropertyKey, false);
                this.SetValue(ErrorContentPropertyKey, null);
                this.RaiseEvent(new ErrorEventArgs(ErrorEvent, null));
            }
        }
    }
}
