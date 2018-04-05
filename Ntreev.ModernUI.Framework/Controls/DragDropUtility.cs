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

namespace Ntreev.ModernUI.Framework.Controls
{
    public static class DragDropUtility
    {
        private const string dropCommandString = "DropCommand";
        private const string dropCommandParameterString = "DropCommandParameter";

        public static readonly DependencyProperty DropCommandProperty =
            DependencyProperty.RegisterAttached(dropCommandString, typeof(ICommand), typeof(DragDropUtility),
                new PropertyMetadata(null, CommandPropertyChangedCallback));

        public static readonly DependencyProperty DropCommandParameterProperty =
            DependencyProperty.RegisterAttached(dropCommandParameterString, typeof(object), typeof(DragDropUtility),
                new PropertyMetadata(null));

        public static ICommand GetDropCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(DropCommandProperty);
        }

        public static void SetDropCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(DropCommandProperty, value);
        }

        public static object GetDropCommandParameter(DependencyObject d)
        {
            return (object)d.GetValue(DropCommandParameterProperty);
        }

        public static void SetDropCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(DropCommandParameterProperty, value);
        }

        public static void ExecuteDropCommand(DependencyObject d, object parameter)
        {
            var command = GetDropCommand(d);

            if (command != null)
            {
                if (command.CanExecute(parameter) == true)
                {
                    command.Execute(parameter);
                }
            }
        }

        public static bool CanExecuteDropCommand(DependencyObject d, object parameter)
        {
            var command = GetDropCommand(d);

            if (command != null)
            {
                return command.CanExecute(parameter);
            }
            return false;
        }

        private static void CommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }
    }
}
