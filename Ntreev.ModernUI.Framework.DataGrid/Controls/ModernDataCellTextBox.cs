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
using System.Windows.Input;
using Xceed.Wpf.Controls;

namespace Ntreev.ModernUI.Framework.Controls
{
    class ModernDataCellTextBox : AutoSelectTextBox
    {
        public ModernDataCellTextBox()
        {
            this.CommandBindings.Add(new CommandBinding(ModernDataGridCommands.NewLine, this.NewLine_Execute, this.NewLine_CanExecute));
        }

        private void NewLine_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var first = this.Text.Substring(0, this.CaretIndex);
            var last = this.Text.Substring(this.CaretIndex + this.SelectedText.Length);
            var index = this.CaretIndex;
            this.Text = first + Environment.NewLine + last;
            this.CaretIndex = index + Environment.NewLine.Length;
        }

        private void NewLine_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}
