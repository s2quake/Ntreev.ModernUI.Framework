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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class BrowserExpander : Expander
    {
        //public static readonly DependencyProperty ToolsProperty =
        //    DependencyProperty.Register(nameof(Tools), typeof(FrameworkElement), typeof(BrowserExpander),
        //        new UIPropertyMetadata(null));

        public static readonly DependencyProperty IsProgressingProperty =
            DependencyProperty.Register(nameof(IsProgressing), typeof(bool), typeof(BrowserExpander),
                new UIPropertyMetadata(true));

        //public static readonly DependencyProperty CloseCommandProperty =
        //    DependencyProperty.Register(nameof(CloseCommand), typeof(ICommand), typeof(BrowserExpander));

        public BrowserExpander()
        {
            //this.DataContextChanged += BrowserExpander_DataContextChanged;
        }

        //public FrameworkElement Tools
        //{
        //    get { return (FrameworkElement)GetValue(ToolsProperty); }
        //    set { SetValue(ToolsProperty, value); }
        //}

        public bool IsProgressing
        {
            get { return (bool)this.GetValue(IsProgressingProperty); }
            set { this.SetValue(IsProgressingProperty, value); }
        }

        //public ICommand CloseCommand
        //{
        //    get { return (ICommand)this.GetValue(CloseCommandProperty); }
        //    set { this.SetValue(CloseCommandProperty, value); }
        //}

        //private void BrowserExpander_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (this.Tools != null)
        //    {
        //        Bind.SetModel(this.Tools, this.DataContext);
        //    }
        //}
    }
}
