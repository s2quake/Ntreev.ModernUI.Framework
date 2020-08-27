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
using System.Windows.Markup;
using Xceed.Wpf.DataGrid;

namespace JSSoft.ModernUI.Framework.DataGrid.Markup
{
    [MarkupExtensionReturnType(typeof(object))]
    public class ContentBindingExtension : MarkupExtension
    {
        private readonly Binding binding = new Binding();

        public ContentBindingExtension()
        {
            this.binding.RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor) { AncestorType = typeof(Cell), };
            this.binding.Mode = BindingMode.OneWay;
            this.binding.Path = new PropertyPath("Content");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.binding.ProvideValue(serviceProvider);
        }

        public object TargetNullValue
        {
            get => this.binding.TargetNullValue;
            set => this.binding.TargetNullValue = value;
        }
    }
}
