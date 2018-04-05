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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class SimpleWindow : ModernWindow
    {
        public static readonly DependencyProperty ToolsProperty =
            DependencyProperty.Register("Tools", typeof(ToolBar), typeof(SimpleWindow),
                new FrameworkPropertyMetadata(null));

        public SimpleWindow()
        {
            this.DataContextChanged += SimpleWindow_DataContextChanged;
        }

        public ToolBar Tools
        {
            get { return (ToolBar)this.GetValue(ToolsProperty); }
            set { this.SetValue(ToolsProperty, value); }
        }

        private void SimpleWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Tools != null)
            {
                Caliburn.Micro.Bind.SetModel(this.Tools, e.NewValue);
            }
        }
    }
}
