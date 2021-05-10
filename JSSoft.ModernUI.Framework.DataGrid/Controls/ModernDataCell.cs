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
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public class ModernDataCell : DataCell
    {
        public static readonly DependencyProperty EditingContentProperty =
            DependencyProperty.Register(nameof(EditingContent), typeof(object), typeof(ModernDataCell),
                new UIPropertyMetadata(null, EditingContentPropertyChangedCallback));

        public ModernDataCell()
        {
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToNextItem, this.MoveToNextItem_Execute, this.MoveToNextItem_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToPrevItem, this.MoveToPrevItem_Execute, this.MoveToPrevItem_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToNextColumn, this.MoveToNextColumn_Execute, this.MoveToNextColumn_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(ModernDataGridCommands.MoveToPrevColumn, this.MoveToPrevColumn_Execute, this.MoveToPrevColumn_CanExecute));
        }

        public object EditingContent
        {
            get => (object)GetValue(EditingContentProperty);
            set => SetValue(EditingContentProperty, value);
        }

        public bool HasDataContextError => ModernDataGridControl.GetHasDataContextError(this);

        public object DataContextError => ModernDataGridControl.GetDataContextError(this);

        public ModernDataGridControl GridControl => (ModernDataGridControl)this.GridContext.DataGridControl;

        public DataGridContext GridContext => (DataGridContext)this.GetValue(DataGridControl.DataGridContextProperty);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var be = this.GetBindingExpression(Cell.ContentProperty);
            if (be != null)
            {
                be.ParentBinding.UpdateSourceExceptionFilter = this.UpdateSourceExceptionFilterCallback;
            }
        }

        protected override void InitializeCore(DataGridContext dataGridContext, Row parentRow, ColumnBase parentColumn)
        {
            base.InitializeCore(dataGridContext, parentRow, parentColumn);

            if (parentRow is ModernDataRow dataRow)
            {
                dataRow.DataContextErrorChanged += DataRow_DataContextErrorChanged;
            }
        }

        private void DataRow_DataContextErrorChanged(object sender, EventArgs e)
        {
            this.RefreshDataContextError();
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);

            this.RefreshDataContextError();
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
        }

        protected override void OnPreviewMouseRightButtonDown(MouseButtonEventArgs e)
        {
            if (this.ContextMenu != null || this.ParentRow.ContextMenu != null)
            {
                this.Focus();
            }
            base.OnPreviewMouseRightButtonDown(e);
        }

        protected override void OnEditBeginning(CancelRoutedEventArgs e)
        {
            base.OnEditBeginning(e);
        }

        protected override void OnEditEnded()
        {
            base.OnEditEnded();
            this.EditingContent = this.Content;
        }

        protected override void OnEditEnding(CancelRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as ModernDataGridControl;

            if (gridControl.InvokeValueChangingEvent(this, this.EditingContent) == true)
            {
                this.Content = this.EditingContent;
            }
            else
            {
                //this.EditingContent = this.Content;
            }
            base.OnEditEnding(e);
        }

        protected override void OnEditCanceled()
        {
            base.OnEditCanceled();
            this.EditingContent = this.Content;
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            this.EditingContent = newContent;
        }

        protected void RefreshDataContextError()
        {
            if (this.DataContext is IDataErrorInfo errorInfo && this.FieldName != string.Empty && errorInfo[this.FieldName] != string.Empty)
            {
                ModernDataGridControl.SetDataContextError(this, errorInfo[this.FieldName]);
            }
            else
            {
                ModernDataGridControl.SetDataContextError(this, null);
            }

        }

        private object UpdateSourceExceptionFilterCallback(object bindExpression, Exception exception)
        {
            throw new ModernDataGridUpdateSourceException(this, exception);
        }

        private void MoveToNextColumn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToNextColumn_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.EndEditAction() == false)
                return;

            var gridContext = DataGridControl.GetDataGridContext(this);
            var index = gridContext.VisibleColumns.IndexOf(this.ParentColumn);
            if (gridContext.VisibleColumns.Count != (index + 1))
            {
                var newColumn = gridContext.VisibleColumns[index + 1];
                var item = gridContext.CurrentItem;
                gridContext.SelectedCellRanges.Clear();
                gridContext.CurrentItem = null;
                gridContext.CurrentItem = item;
                gridContext.CurrentColumn = newColumn;
                gridContext.FocusCurrent();
            }
        }

        private void MoveToPrevColumn_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToPrevColumn_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.EndEditAction() == false)
                return;

            var gridContext = DataGridControl.GetDataGridContext(this);
            var index = gridContext.VisibleColumns.IndexOf(this.ParentColumn);
            if (index != 0)
            {
                var newColumn = gridContext.VisibleColumns[index - 1];
                var item = gridContext.CurrentItem;
                gridContext.SelectedCellRanges.Clear();
                gridContext.CurrentItem = null;
                gridContext.CurrentItem = item;
                gridContext.CurrentColumn = newColumn;
                gridContext.FocusCurrent();
            }
        }

        private void MoveToNextItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToNextItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.EndEditAction() == false)
                return;

            var gridContext = DataGridControl.GetDataGridContext(this);
            var index = gridContext.Items.IndexOf(this.DataContext);
            if (index + 1 < gridContext.Items.Count)
            {
                var newItem = gridContext.Items.GetItemAt(index + 1);
                gridContext.SelectedCellRanges.Clear();
                gridContext.CurrentItem = newItem;
                gridContext.FocusCurrent();
            }
        }

        private void MoveToPrevItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void MoveToPrevItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (this.EndEditAction() == false)
                return;

            var gridContext = DataGridControl.GetDataGridContext(this);
            if (gridContext.Items.IndexOf(this.DataContext) > 0)
            {
                var index = gridContext.Items.IndexOf(this.DataContext);
                var newItem = gridContext.Items.GetItemAt(index - 1);
                gridContext.SelectedCellRanges.Clear();
                gridContext.CurrentItem = newItem;
                gridContext.FocusCurrent();
            }
        }

        private static void EditingContentPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private bool EndEditAction()
        {
            if (this.IsBeingEdited == true)
            {
                try
                {
                    this.EndEdit();
                }
                catch (Xceed.Wpf.DataGrid.DataGridException ex)
                {
                    if (ex.InnerException != null)
                        AppMessageBox.ShowErrorAsync(ex.InnerException.Message);
                    else
                        AppMessageBox.ShowErrorAsync(ex);
                    return false;
                }
                catch (Exception ex)
                {
                    AppMessageBox.ShowErrorAsync(ex);
                    return false;
                }
            }

            return true;
        }

    }
}
