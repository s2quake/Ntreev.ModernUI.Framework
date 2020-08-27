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
using System.Windows.Data;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public class TimeSpanUpDown : Xceed.Wpf.Toolkit.TimeSpanUpDown
    {
        public static readonly DependencyProperty InternalValueProperty =
            DependencyProperty.Register(nameof(InternalValue), typeof(TimeSpan?), typeof(TimeSpanUpDown));

        public TimeSpanUpDown()
        {
            BindingOperations.SetBinding(this, ValueProperty, new Binding()
            {
                Path = new PropertyPath(InternalValueProperty),
                RelativeSource = new RelativeSource(RelativeSourceMode.Self),
                Mode = BindingMode.TwoWay,
            });
        }

        public TimeSpan? InternalValue
        {
            get => (TimeSpan?)this.GetValue(InternalValueProperty);
            set => this.SetValue(InternalValueProperty, value);
        }

        protected override void OnTextChanged(string previousValue, string currentValue)
        {
            if (TimeSpan.TryParse(currentValue, out _) == false)
                return;
            base.OnTextChanged(previousValue, currentValue);
        }
    }
}
