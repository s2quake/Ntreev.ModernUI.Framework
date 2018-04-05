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

using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = "PART_ProgressContent", Type = typeof(ProgressContent))]
    public class DialogWindow : ModernWindow
    {
        public static readonly DependencyProperty ProgressStyleProperty =
            DependencyProperty.RegisterAttached(nameof(ProgressStyle), typeof(Style), typeof(DialogWindow));

        public static readonly DependencyProperty ProgressTypeProperty =
            DependencyProperty.RegisterAttached(nameof(ProgressType), typeof(ProgressType), typeof(DialogWindow), 
                new UIPropertyMetadata(ProgressType.Ring));

        private static readonly DependencyProperty ProgressTypeInternalProperty =
            DependencyProperty.RegisterAttached("ProgressTypeInternal", typeof(ProgressType), typeof(DialogWindow));

        public static readonly DependencyProperty ProgressMessageProperty =
            DependencyProperty.Register("ProgressMessage", typeof(string), typeof(DialogWindow), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsProgressingProperty =
            DependencyProperty.Register("IsProgressing", typeof(bool), typeof(DialogWindow));

        private static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.RegisterAttached("DesiredWidth", typeof(double), typeof(DialogWindow),
                new FrameworkPropertyMetadata(double.NaN, DesiredWidthPropertyChangedCallback));

        private static readonly DependencyProperty DesiredHeightProperty =
            DependencyProperty.RegisterAttached("DesiredHeight", typeof(double), typeof(DialogWindow),
                new FrameworkPropertyMetadata(double.NaN, DesiredHeightPropertyChangedCallback));

        internal static readonly DependencyProperty DesiredResizeModeProperty =
            DependencyProperty.RegisterAttached("DesiredResizeMode", typeof(ResizeMode), typeof(DialogWindow),
                new FrameworkPropertyMetadata(ResizeMode.NoResize));

        private ProgressContent progressContent;

        public DialogWindow()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.progressContent = this.Template.FindName("PART_ProgressContent", this) as ProgressContent;
        }

        public static Style GetProgressStyle(Control control)
        {
            return (Style)control.GetValue(ProgressStyleProperty);
        }

        public static void SetProgressStyle(Control control, Style value)
        {
            control.SetValue(ProgressStyleProperty, value);
        }

        public static ProgressType GetProgressType(Control control)
        {
            return (ProgressType)control.GetValue(ProgressTypeProperty);
        }

        public static void SetProgressType(Control control, ProgressType value)
        {
            control.SetValue(ProgressTypeProperty, value);
        }

        public static double GetDesiredWidth(FrameworkElement control)
        {
            return (double)control.GetValue(DesiredWidthProperty);
        }

        public static void SetDesiredWidth(FrameworkElement control, double value)
        {
            control.SetValue(DesiredWidthProperty, value);
        }

        public static double GetDesiredHeight(FrameworkElement fe)
        {
            return (double)fe.GetValue(DesiredHeightProperty);
        }

        public static void SetDesiredHeight(FrameworkElement fe, double value)
        {
            fe.SetValue(DesiredHeightProperty, value);
        }

        public static ResizeMode GetDesiredResizeMode(FrameworkElement fe)
        {
            return (ResizeMode)fe.GetValue(DesiredResizeModeProperty);
        }

        public static void SetDesiredResizeMode(FrameworkElement fe, ResizeMode value)
        {
            fe.SetValue(DesiredResizeModeProperty, value);
        }

        public bool IsProgressing
        {
            get { return (bool)this.GetValue(IsProgressingProperty); }
            set { this.SetValue(IsProgressingProperty, value); }
        }

        public bool ProgressMessage
        {
            get { return (bool)this.GetValue(ProgressMessageProperty); }
            set { this.SetValue(ProgressMessageProperty, value); }
        }

        public ProgressType ProgressType
        {
            get { return (ProgressType)this.GetValue(ProgressTypeProperty); }
            set { this.SetValue(ProgressTypeProperty, value); }
        }

        public Style ProgressStyle
        {
            get { return (Style)this.GetValue(ProgressStyleProperty); }
            set { this.SetValue(ProgressStyleProperty, value); }
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (newContent is DialogContentControl dialogContent)
            {
                if (dialogContent.Content is Control control)
                {
                    this.ProgressType = GetProgressType(control);
                    if (this.ProgressType == ProgressType.Custom)
                    {
                        this.ProgressStyle = GetProgressStyle(control);
                    }
                    this.ResizeMode = GetDesiredResizeMode(control);
                }
            }
            else if (newContent is Control control)
            {
                this.ProgressType = GetProgressType(control);
                if (this.ProgressType == ProgressType.Custom)
                {
                    this.ProgressStyle = GetProgressStyle(control);
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var size = base.MeasureOverride(availableSize);
            if (this.Content is FrameworkElement)
            {
                var desiredWidth = GetDesiredWidth(this.Content as FrameworkElement);
                var desiredHeight = GetDesiredHeight(this.Content as FrameworkElement);
                if (double.IsNaN(desiredWidth) == false)
                    size.Width = desiredWidth;
                if (double.IsNaN(desiredHeight) == false)
                    size.Height = desiredHeight;
            }
            return size;
        }

        private static void DesiredWidthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d) == true)
            {
                d.SetValue(FrameworkElement.WidthProperty, e.NewValue);
            }
        }

        private static void DesiredHeightPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d) == true)
            {
                d.SetValue(FrameworkElement.HeightProperty, e.NewValue);
            }
        }
    }
}
