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

using Ntreev.ModernUI.Framework.DataGrid.Controls;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.DataGrid.Assets
{
    partial class ModernDataGridControl : ResourceDictionary
    {
        public ModernDataGridControl()
        {

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.Tag is DataCell cell && cell.IsBeingEdited == true)
                cell.EndEdit();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.Tag is DataCell cell && cell.IsBeingEdited == true)
                cell.EndEdit();
        }

        private void ColumnManagerCell_MouseLeftButtonUp(object sender, MouseEventArgs e)
        {

        }

        private void HierarchicalGroupByControl_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as HierarchicalGroupByControl;
            if (control.Template.FindName("rootBorder", control) is Border border)
                border.BorderThickness = new Thickness(0);
        }

        private void HierarchicalGroupByItem_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void ColumnManagerRowSelector_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var rowSelector = sender as RowSelector;
            var gridContext = DataGridControl.GetDataGridContext(rowSelector.DataContext as DependencyObject);
            var gridControl = gridContext.DataGridControl as Controls.ModernDataGridControl;
            if (gridControl.SelectionMode != SelectionMode.Single)
                gridControl.SelectAll(gridContext);
        }

        private void SynchronizedScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {


        }

        private void Control_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void GroupLevelIndicator_IsLoaded(object sender, RoutedEventArgs e)
        {
            var indicator = sender as GroupLevelIndicator;
            var panel = indicator.Parent as Panel;
            int index = panel.Children.IndexOf(indicator);
            indicator.Background = new SolidColorBrush(Controls.ModernDataGridControl.GetColor(index)) { Opacity = 0.1f, };
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox.Tag is ModernDataCell cell && cell.IsBeingEdited == true)
            {
                cell.EndEdit();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as System.Windows.Controls.ComboBox;
            if (comboBox.Tag is ModernDataCell cell && cell.IsBeingEdited == true)
            {
                cell.EndEdit();
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {

        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {

        }

        private void StringTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (Keyboard.Modifiers == ModifierKeys.Alt && e.Key == Key.Enter)
                {
                    textBox.Text += Environment.NewLine;
                }
                else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control)
                {
                    textBox.AcceptsReturn = true;
                    textBox.Dispatcher.InvokeAsync(() => textBox.AcceptsReturn = false, DispatcherPriority.DataBind);
                }
            }
        }

        private void DateTimePicker_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var picker = sender as Xceed.Wpf.Toolkit.DateTimePicker;
            picker.CommitInput();
        }

        private void DateTimePicker_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None && e.Key == Key.Escape)
            {
                var picker = sender as Xceed.Wpf.Toolkit.DateTimePicker;
                var be = BindingOperations.GetBindingExpression(picker, Xceed.Wpf.Toolkit.DateTimePicker.ValueProperty);
                picker.CommitInput();
                be.UpdateTarget();
            }
            else if (e.Key == Key.Tab)
            {
                var picker = sender as Xceed.Wpf.Toolkit.DateTimePicker;
                picker.CommitInput();
            }
        }
    }
}
