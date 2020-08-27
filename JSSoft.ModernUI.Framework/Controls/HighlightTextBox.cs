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
using System.Windows.Documents;
using System.Windows.Media;

namespace JSSoft.ModernUI.Framework.Controls
{
    public class HighlightTextBox : System.Windows.Controls.RichTextBox
    {
        public static readonly DependencyProperty FilterPatternProperty =
            DependencyProperty.Register(nameof(FilterPattern), typeof(string), typeof(HighlightTextBox),
                new FrameworkPropertyMetadata(string.Empty, FilterPatternPropertyChangedCallback));

        public static readonly DependencyProperty HighlightProperty =
            DependencyProperty.Register(nameof(Highlight), typeof(Brush), typeof(HighlightTextBox),
                new FrameworkPropertyMetadata(null, HighlightPropertyChangedCallback));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(HighlightTextBox),
                new FrameworkPropertyMetadata(string.Empty, TextPropertyChangedCallback));

        public HighlightTextBox()
        {

        }

        public string FilterPattern
        {
            get => (string)this.GetValue(FilterPatternProperty);
            set => this.SetValue(FilterPatternProperty, value);
        }

        public Brush Highlight
        {
            get => (Brush)this.GetValue(HighlightProperty);
            set => this.SetValue(HeightProperty, value);
        }

        public string Text
        {
            get => (string)this.GetValue(TextProperty);
            set => this.SetValue(TextProperty, value);
        }

        private static void FilterPatternPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBox;
            SetText(self.Document, self.Text ?? string.Empty, self.FilterPattern, self.Highlight);
        }

        private static void HighlightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBox;
            SetText(self.Document, self.Text ?? string.Empty, self.FilterPattern, self.Highlight);
        }

        private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as HighlightTextBox;
            SetText(self.Document, self.Text ?? string.Empty, self.FilterPattern, self.Highlight);
        }

        private static void SetText(System.Windows.Documents.FlowDocument document, string text, string filter, Brush highlight)
        {
            document.Blocks.Clear();
            if (filter == string.Empty)
            {
                var range = new TextRange(document.ContentStart, document.ContentEnd)
                {
                    Text = text
                };
                range.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            }
            else
            {
                int index;
                while ((index = text.IndexOf(filter)) >= 0)
                {
                    var t = text.Remove(index);
                    {
                        var range = new TextRange(document.ContentEnd, document.ContentEnd)
                        {
                            Text = t
                        };
                        range.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                    }

                    var f = text.Substring(index, filter.Length);
                    {
                        var range = new TextRange(document.ContentEnd, document.ContentEnd)
                        {
                            Text = f
                        };
                        range.ApplyPropertyValue(TextElement.BackgroundProperty, highlight);
                    }

                    text = text.Substring(index + filter.Length);
                }
                {
                    var range = new TextRange(document.ContentEnd, document.ContentEnd) { Text = text, };
                    range.ApplyPropertyValue(TextElement.BackgroundProperty, null);
                }
            }
        }
    }
}
