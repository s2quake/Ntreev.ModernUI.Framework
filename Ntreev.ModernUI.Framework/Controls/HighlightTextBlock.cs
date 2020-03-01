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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class HighlightTextBlock : TextBlock
    {
        public static readonly DependencyProperty PatternProperty =
            DependencyProperty.Register(nameof(Pattern), typeof(string), typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata(string.Empty, PatternPropertyChangedCallback, PatternCoerceValueCallback));

        public static readonly DependencyProperty CaseSensitiveProperty =
            DependencyProperty.Register(nameof(CaseSensitive), typeof(bool), typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata(false, CaseSensitivePropertyChangedCallback));

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(nameof(Highlight), typeof(Brush), typeof(HighlightTextBlock),
                new FrameworkPropertyMetadata(null, HighlightPropertyChangedCallback));

        public new static DependencyProperty TextProperty =
           DependencyProperty.Register(nameof(Text), typeof(string), typeof(HighlightTextBlock),
               new FrameworkPropertyMetadata(string.Empty, TextPropertyChangedCallback));

        public HighlightTextBlock()
        {

        }

        public string Pattern
        {
            get => (string)this.GetValue(PatternProperty);
            set => this.SetValue(PatternProperty, value);
        }

        public bool CaseSensitive
        {
            get => (bool)this.GetValue(CaseSensitiveProperty);
            set => this.SetValue(CaseSensitiveProperty, value);
        }

        public Brush Highlight
        {
            get => (Brush)this.GetValue(HighlightProperty);
            set => this.SetValue(HeightProperty, value);
        }

        public new string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        private static void PatternPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBlock;
            self.RefreshInlines();
        }

        private static object PatternCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return string.Empty;
            return baseValue;
        }

        private static void HighlightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBlock;
            self.RefreshInlines();
        }

        private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBlock;
            self.RefreshInlines();
        }

        private static void CaseSensitivePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBlock;
            self.RefreshInlines();
        }

        private void RefreshInlines()
        {
            var text = this.Text ?? string.Empty;
            this.Inlines.Clear();
            if (string.IsNullOrEmpty(this.Pattern) == true)
            {
                this.Inlines.Add(text);
            }
            else
            {
                int index = -1;
                var comparison = this.CaseSensitive == false ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
                while ((index = text.IndexOf(this.Pattern, comparison)) >= 0)
                {
                    var t = text.Remove(index);
                    {
                        this.Inlines.Add(new Run(t));
                    }

                    var f = text.Substring(index, this.Pattern.Length);
                    {
                        var run = new Run(f) { Background = this.Highlight, };
                        this.Inlines.Add(run);
                    }

                    text = text.Substring(index + this.Pattern.Length);
                }

                this.Inlines.Add(new Run(text));
            }
        }
    }
}
