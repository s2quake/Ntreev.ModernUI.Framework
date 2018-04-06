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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class ModernInsertionRow : InsertionRow
    {
        private string tableName;
        private string[] columnNames;
        private object[] defaultArray;
        private object[] itemArray;
        private bool hasField;
        private object canCommit;

        public ModernInsertionRow()
        {
            this.InsertionMode = InsertionMode.Continuous;
            this.CommandBindings.Insert(0, new CommandBinding(DataGridCommands.CancelEdit, this.CancelEdit_Execute, this.CancelEdit_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(DataGridCommands.EndEdit, this.EndEdit_Execute, this.EndEdit_CanExecute));
            this.CommandBindings.Insert(0, new CommandBinding(DataGridCommands.EndEdit, this.Insert_Execute, this.Insert_CanExecute));
            this.CellEditorDisplayConditions = Xceed.Wpf.DataGrid.CellEditorDisplayConditions.CellIsCurrent;
        }

        public event EventHandler Inserted;

        internal event EventHandler Detached;

        protected virtual void OnInserted(EventArgs e)
        {
            this.Inserted?.Invoke(this, e);
        }

        protected override Cell CreateCell(ColumnBase column)
        {
            return new ModernInsertionCell();
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            //await this.Dispatcher.InvokeAsync(() =>
            //{

            var gridContext = DataGridControl.GetDataGridContext(this);
            var isBeingEdited = this.Cells[gridContext.CurrentColumn].IsBeingEdited;
            if ((bool)e.NewValue == true)
            {
                if (isBeingEdited == false)
                {
                    gridContext.BeginEdit();
                    //System.Diagnostics.Trace.WriteLine($"begin : {this.IsBeingEdited}");
                }
            }
            else
            {
                //var gridContext = DataGridControl.GetDataGridContext(this);
                if (this.hasField == true && isBeingEdited == false)
                {
                    gridContext.CancelEdit();
                    this.Detached?.Invoke(this, EventArgs.Empty);
                    //System.Diagnostics.Trace.WriteLine($"cancel : {this.IsBeingEdited}");
                }
            }
            //}, DispatcherPriority.Render);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        protected override void BeginEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
                if (this.hasField == false)
                {
                    try
                    {
                        base.BeginEditCore();

                        var gridContext = DataGridControl.GetDataGridContext(this);
                        var typedList = gridContext.Items.SourceCollection as ITypedList;
                        if (typedList == null)
                        {
                            var source = (gridContext.Items.SourceCollection as CollectionView).SourceCollection;
                            typedList = source as ITypedList;
                        }

                        var props = typedList.GetItemProperties(null);

                        this.tableName = typedList.GetListName(null);
                        this.columnNames = GetColumnNames(typedList);

                        this.defaultArray = new object[this.columnNames.Length];
                        this.itemArray = new object[this.columnNames.Length];
                        for (var i = 0; i < this.columnNames.Length; i++)
                        {
                            var prop = props[this.columnNames[i]];
                            if (prop.IsReadOnly == true)
                                continue;
                            var field = prop.GetValue(this.DataContext);
                            if (field != DBNull.Value)
                            {
                                this.defaultArray[i] = field;
                                this.itemArray[i] = field;
                            }
                        }

                        if (gridContext.CurrentColumn != null)
                        {
                            this.Cells[gridContext.CurrentColumn].CancelEdit();
                        }
                    }
                    finally
                    {
                        this.hasField = true;
                    }
                }
                else
                {
                    var gridContext = DataGridControl.GetDataGridContext(this);
                    base.BeginEditCore();
                    if (gridContext.CurrentColumn != null)
                    {
                        this.Cells[gridContext.CurrentColumn].CancelEdit();
                    }
                }
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        protected override void EndEditCore()
        {
            //System.Diagnostics.Trace.WriteLine(nameof(EndEditCore));
            if (this.canCommit != null)
                base.EndEditCore();
            else
                base.CancelEditCore();
        }

        protected override void CancelEditCore()
        {
            try
            {
                this.IsBeginEnding = true;
                base.CancelEditCore();
            }
            finally
            {
                this.IsBeginEnding = false;
            }
        }

        protected override void OnEditEnded()
        {
            base.OnEditEnded();
            if (this.canCommit != null)
            {
                var gridContext = DataGridControl.GetDataGridContext(this);
                gridContext.Items.MoveCurrentTo(this.canCommit);
                this.canCommit = null;
            }
        }

        protected override void OnEditCanceled()
        {
            //System.Diagnostics.Trace.WriteLine(nameof(OnEditCanceled));
            base.OnEditCanceled();
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
            ModernDataGridControl.SetInsertionRow(dataGridContext, this);
        }

        private void CancelEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            if (gridContext.CurrentColumn == null)
                return;

            var cell = this.Cells[gridContext.CurrentColumn];
            if (cell.IsBeingEdited == false)
                e.CanExecute = true;
        }

        private void CancelEdit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            //var gridContext = DataGridControl.GetDataGridContext(this);
            //var cell = this.Cells[gridContext.CurrentColumn];
            //cell.CancelEdit();
        }

        private void EndEdit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            if (gridContext.CurrentColumn == null)
                return;

            var cell = this.Cells[gridContext.CurrentColumn];
            if (cell.IsBeingEdited == true)
                e.CanExecute = true;
        }

        private void EndEdit_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var cell = this.Cells[gridContext.CurrentColumn];
            cell.EndEdit();
        }

        private void Insert_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            if (gridContext.CurrentColumn == null)
                return;

            var cell = this.Cells[gridContext.CurrentColumn];
            if (cell.IsBeingEdited == false)
                e.CanExecute = true;
        }

        private void Insert_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var gridContext = DataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as ModernDataGridControl;

            if (gridControl.InvokeItemInsertingEvent(this) == true)
            {
                try
                {
                    this.canCommit = this.DataContext;
                    this.EndEdit();
                    this.tableName = null;
                    this.columnNames = null;
                    this.defaultArray = null;
                    this.itemArray = null;
                    this.hasField = false;
                    this.canCommit = null;
                    this.OnInserted(EventArgs.Empty);
                }
                catch (DataGridException ex)
                {
                    if (ex.InnerException != null)
                        AppMessageBox.ShowError(ex.InnerException.Message);
                    else
                        AppMessageBox.ShowError(ex.Message);
                }
                catch (Exception ex)
                {
                    AppMessageBox.ShowError(ex.Message);
                }
                finally
                {
                    this.canCommit = null;
                }
            }
        }

        private object[] CreateFields()
        {
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                var cell = this.Cells[this.columnNames[i]];
                if (cell == null || cell.ReadOnly == true || cell.Content == null)
                    continue;

                this.itemArray[i] = cell.Content;
            }
            return this.itemArray;
        }

        internal void SetField(string fieldName, object content)
        {
            if (this.columnNames == null)
                return;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    this.itemArray[i] = content;
                }
            }
        }

        internal object GetField(string fieldName)
        {
            if (this.columnNames == null)
                return null;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    return this.itemArray[i];
                }
            }
            return null;
        }

        internal bool HasField => this.hasField;

        internal void ResetField(string fieldName)
        {
            if (this.columnNames == null)
                return;
            for (var i = 0; i < this.columnNames.Length; i++)
            {
                if (this.columnNames[i] == fieldName)
                {
                    this.itemArray[i] = this.defaultArray[i];
                }
            }
        }

        internal bool IsBeginEnding
        {
            get;
            set;
        }

        public static string[] GetColumnNames(ITypedList typedList)
        {
            var props = typedList.GetItemProperties(null);
            var columnList = new List<string>(props.Count);
            for (var i = 0; i < props.Count; i++)
            {
                if (props[i].PropertyType == typeof(IBindingList))
                    continue;
                columnList.Add(props[i].Name);
            }
            return columnList.ToArray();
        }
    }
}
