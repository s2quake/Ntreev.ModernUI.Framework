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
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.Controls
{
    public class ModernColumnManagerCell : ColumnManagerCell
    {
        public static readonly DependencyProperty IsCurrentColumnProperty =
            DependencyProperty.Register(nameof(IsCurrentColumn), typeof(bool), typeof(ModernColumnManagerCell), new PropertyMetadata(false));

        public static readonly DependencyProperty HasSelectedProperty =
            DependencyProperty.Register(nameof(HasSelected), typeof(bool), typeof(ModernColumnManagerCell));

        private Point? mousePoint;

        public void SetHasSelected(DataGridContext gridContext)
        {
            int index = this.ParentColumn.VisiblePosition;
            bool hasSelected = false;

            foreach (var item in gridContext.SelectedCellRanges)
            {
                if (item.ColumnRange.Contains(index) == true)
                {
                    hasSelected = true;
                    break;
                }
            }
            this.HasSelected = hasSelected;
        }

        public bool IsCurrentColumn
        {
            get { return (bool)GetValue(IsCurrentColumnProperty); }
            set { SetValue(IsCurrentColumnProperty, value); }
        }

        public bool HasSelected
        {
            get { return (bool)GetValue(HasSelectedProperty); }
            set { SetValue(HasSelectedProperty, value); }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var index = this.ParentColumn.VisiblePosition;
            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl as ModernDataGridControl;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                gridControl.AddSelection(this);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                gridControl.SelectTo(this);
                e.Handled = true;
            }
            else if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                gridControl.AddSelectionTo(this);
                e.Handled = true;
            }
            else
            {
                if (gridControl.GlobalCurrentColumn == this.ParentColumn)
                {
                    this.mousePoint = e.GetPosition(this);
                }
                else if (gridControl.SelectionUnit == SelectionUnit.Cell)
                {
                    gridControl.Select(this);
                }
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            if (this.mousePoint.HasValue && this.mousePoint.Value == e.GetPosition(this))
            {
                if (this.ParentColumn.AllowSort == true)
                {
                    var gridContext = ModernDataGridControl.GetDataGridContext(this);
                    this.DoSort(gridContext.Items);
                }
            }
            this.mousePoint = null;
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
        }

        private void DoSort(CollectionView items)
        {
            var column = this.ParentColumn as Column;
            var index = column.SortDirectionCycle.IndexOf(column.SortDirection);
            var nextIndex = (index + 1) % column.SortDirectionCycle.Count;
            var sortDirection = column.SortDirectionCycle[nextIndex];

            items.SortDescriptions.Clear();
            if (sortDirection == SortDirection.Ascending)
            {
                items.SortDescriptions.Add(new SortDescription(column.FieldName, ListSortDirection.Ascending));
            }
            else if (sortDirection == SortDirection.Descending)
            {
                items.SortDescriptions.Add(new SortDescription(column.FieldName, ListSortDirection.Descending));
            }
        }
    }
}
