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
using System.Windows;
using System.Windows.Controls;

namespace Ntreev.ModernUI.Framework.Controls
{
    class DialogContentControl : ContentControl
    {
        public DialogContentControl()
        {
            this.DataContextChanged += DialogContentControl_DataContextChanged;
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            if (this.Content is FrameworkElement)
            {
                var desiredWidth = DialogWindow.GetDesiredWidth(this.Content as FrameworkElement);
                var desiredHeight = DialogWindow.GetDesiredHeight(this.Content as FrameworkElement);
                if (this.Parent is DialogWindow window && window.IsEnsured == false)
                {
                    if (double.IsNaN(desiredWidth) == false)
                    {
                        size.Width = Math.Min(constraint.Width, desiredWidth);
                    }
                    if (double.IsNaN(desiredHeight) == false)
                    {
                        size.Height = Math.Min(constraint.Height, desiredHeight);
                    }
                }
            }
            return size;
        }

        private void DialogContentControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Content is DependencyObject)
            {
                Caliburn.Micro.Bind.SetModel(this.Content as DependencyObject, e.NewValue);
            }
        }
    }
}
