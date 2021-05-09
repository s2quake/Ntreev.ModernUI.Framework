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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace JSSoft.ModernUI.Framework.Controls
{
    public static class TextBlockService
    {
        private const string ShowTrimmedText = nameof(ShowTrimmedText);
        private const string IsTextTrimmed = nameof(IsTextTrimmed);

        public static readonly DependencyProperty ShowTrimmedTextProperty =
            DependencyProperty.RegisterAttached(ShowTrimmedText, typeof(bool), typeof(TextBlockService),
                new UIPropertyMetadata(false, ShowTrimmedTextPropertyChangedCallback));

        private static readonly DependencyPropertyKey IsTextTrimmedPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(IsTextTrimmed, typeof(bool), typeof(TextBlockService),
                new UIPropertyMetadata(false));
        public static readonly DependencyProperty IsTextTrimmedProperty = IsTextTrimmedPropertyKey.DependencyProperty;

        private static readonly HashSet<TextBlock> textBlocks = new();

        private static void ShowTrimmedTextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                if ((bool)e.OldValue == true)
                {
                    textBlock.SizeChanged -= TextBlock_SizeChanged;
                }

                if ((bool)e.NewValue == true)
                {
                    textBlock.SetValue(IsTextTrimmedPropertyKey, IsTextBlockTrimmed(textBlock));
                    textBlock.SizeChanged += TextBlock_SizeChanged;
                    textBlock.SetValue(IsTextTrimmedPropertyKey, IsTextBlockTrimmed(textBlock));
                }
            }
        }

        private static void TextBlock_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                if (textBlocks.Contains(textBlock) == false)
                {
                    textBlock.Dispatcher.InvokeAsync(() =>
                    {
                        textBlock.SetValue(IsTextTrimmedPropertyKey, IsTextBlockTrimmed(textBlock));
                        textBlocks.Remove(textBlock);
                    }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                    textBlocks.Add(textBlock);
                }
            }
        }

        private static void TextBlock_LayoutUpdated(object sender, EventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                textBlock.SetValue(IsTextTrimmedPropertyKey, IsTextBlockTrimmed(textBlock));
            }
        }

        public static bool GetShowTrimmedText(TextBlock d)
        {
            return (bool)d.GetValue(ShowTrimmedTextProperty);
        }

        public static void SetShowTrimmedText(TextBlock d, bool value)
        {
            d.SetValue(ShowTrimmedTextProperty, value);
        }

        public static bool GetIsTextTrimmed(TextBlock d)
        {
            return (bool)d.GetValue(IsTextTrimmedProperty);
        }

        private static bool IsTextBlockTrimmed(TextBlock d)
        {
            if (d.TextTrimming == TextTrimming.None)
                return false;
            if (d.ActualWidth == 0)
                return false;
            d.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            return d.ActualWidth < d.DesiredSize.Width;
        }
    }
}
