//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using JSSoft.ModernUI.Framework.Converters;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace JSSoft.ModernUI.Framework.Markup
{
    [MarkupExtensionReturnType(typeof(Visibility))]
    public class VisibilityBindingExtension : MarkupExtension
    {
        private readonly BooleanToVisibilityConverter converter;
        private readonly Binding binding;

        public VisibilityBindingExtension()
            : this(null)
        {

        }

        public VisibilityBindingExtension(string path)
        {
            this.converter = new BooleanToVisibilityConverter();
            this.binding = new Binding()
            {
                Converter = converter,
            };

            if (path != null)
                this.binding.Path = Path = new PropertyPath(path);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.binding.ProvideValue(serviceProvider);
        }

        [DefaultValue(null)]
        public PropertyPath Path
        {
            get => this.binding.Path;
            set => this.binding.Path = value;
        }

        public bool Inverse
        {
            get => this.converter.IsInversed;
            set => this.converter.IsInversed = value;
        }

        public bool IsHidden
        {
            get => this.converter.IsHidden;
            set => this.converter.IsHidden = value;
        }

        public string ElementName
        {
            get => this.binding.ElementName;
            set => this.binding.ElementName = value;
        }

        public BindingMode Mode
        {
            get => this.binding.Mode;
            set => this.binding.Mode = value;
        }

        public RelativeSource RelativeSource
        {
            get => this.binding.RelativeSource;
            set => this.binding.RelativeSource = value;
        }
    }
}
