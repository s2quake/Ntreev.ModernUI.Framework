﻿// Released under the MIT License.
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
using System.Windows.Data;

namespace JSSoft.ModernUI.Framework
{
    public static class PropertyPathHelper
    {
        public static object GetValue(object obj, string propertyPath)
        {
            var binding = new Binding(propertyPath)
            {
                Mode = BindingMode.OneTime,
                Source = obj
            };
            BindingOperations.SetBinding(dummy, DummyObject.ValueProperty, binding);
            return dummy.GetValue(DummyObject.ValueProperty);
        }

        private static readonly DummyObject dummy = new();

        #region class

        private class DummyObject : DependencyObject
        {
            public static readonly DependencyProperty ValueProperty =
                DependencyProperty.Register(nameof(Value), typeof(object), typeof(DummyObject), new UIPropertyMetadata(null));

            public object Value
            {
                get => (object)this.GetValue(ValueProperty);
                set => this.SetValue(ValueProperty, value);
            }
        }

        #endregion
    }
}
