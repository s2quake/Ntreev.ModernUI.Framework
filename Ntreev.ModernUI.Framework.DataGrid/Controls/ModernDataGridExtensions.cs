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
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    public static class ModernDataGridExtensions
    {
        public static bool Contains(this SelectionRange range, int index)
        {
            if (range.StartIndex <= range.EndIndex)
                return index >= range.StartIndex && index <= range.EndIndex;
            return index >= range.EndIndex && index <= range.StartIndex;
        }

        public static ModernDataCell GetCurrentCell(this DataGridContext gridContext)
        {
            if (gridContext.CurrentItem == null || gridContext.CurrentColumn == null)
                return null;

            var row = gridContext.GetContainerFromItem(gridContext.CurrentItem) as ModernDataRow;
            if (row == null)
                return null;

            return row.Cells[gridContext.CurrentColumn] as ModernDataCell;
        }

        public static bool FocusCurrent(this DataGridContext gridContext)
        {
            if (gridContext.CurrentItem != null && gridContext.CurrentColumn != null)
            {
                if (gridContext.GetContainerFromItem(gridContext.CurrentItem) is ModernDataRow row)
                {
                    var cell = row.Cells[gridContext.CurrentColumn];
                    if (cell.IsFocused == false)
                        return cell.Focus();
                }
            }
            return false;
        }

        public static void SelectItem(this DataGridContext gridContext, object item)
        {
            var gridControl = gridContext.DataGridControl;
            if (gridControl.SelectionUnit == SelectionUnit.Row)
            {


            }
            else
            {
                gridContext.SelectedCellRanges.Clear();
                var index = gridContext.Items.IndexOf(item);
                var columnCount = gridContext.VisibleColumns.Count;
                var range = new SelectionCellRange(new SelectionRange(index), new SelectionRange(0, columnCount - 1));
                gridContext.SelectedCellRanges.Add(range);
            }
        }

        public static IEnumerable<object> EnumerateItems(this DataGridContext gridContext, SelectionRange itemRange)
        {
            var startIndex = Math.Min(itemRange.StartIndex, itemRange.EndIndex);
            var endIndex = Math.Max(itemRange.StartIndex, itemRange.EndIndex);
            for (var i = startIndex; i <= endIndex; i++)
            {
                yield return gridContext.Items.GetItemAt(i);
            }
        }

        public static IEnumerable<string> EnumerateFields(this DataGridContext gridContext, object item, SelectionRange columnRange)
        {
            var startIndex = Math.Min(columnRange.StartIndex, columnRange.EndIndex);
            var endIndex = Math.Max(columnRange.StartIndex, columnRange.EndIndex);
            for (var i = startIndex; i <= endIndex; i++)
            {
                foreach (var column in gridContext.Columns)
                {
                    if (column.VisiblePosition == i)
                        yield return column.FieldName;
                }
            }
        }

        public static IEnumerable<ColumnBase> EnumerateColumns(this DataGridContext gridContext, SelectionRange columnRange)
        {
            var startIndex = Math.Min(columnRange.StartIndex, columnRange.EndIndex);
            var endIndex = Math.Max(columnRange.StartIndex, columnRange.EndIndex);
            for (var i = startIndex; i <= endIndex; i++)
            {
                foreach (var column in gridContext.Columns)
                {
                    if (column.VisiblePosition == i)
                        yield return column;
                }
            }
        }

        public static IEnumerable<object> GetSelectedItems(this DataGridContext gridContext)
        {
            if (gridContext.DataGridControl.SelectionUnit == SelectionUnit.Cell)
            {
                var query = from range in gridContext.SelectedCellRanges
                            from item in gridContext.EnumerateItems(range.ItemRange)
                            select item;

                return query.Distinct().OrderBy(item => gridContext.Items.IndexOf(item));
            }
            else
            {
                var query = from range in gridContext.SelectedItemRanges
                            from item in gridContext.EnumerateItems(range)
                            select item;
                return query.Distinct().OrderBy(item => gridContext.Items.IndexOf(item));
            }
        }

        public static IEnumerable<ColumnBase> GetSelectedColumns(this DataGridContext gridContext)
        {
            var query = from range in gridContext.SelectedCellRanges
                        from item in gridContext.EnumerateColumns(range.ColumnRange)
                        select item;

            return query.Distinct().OrderBy(item => gridContext.VisibleColumns.IndexOf(item));
        }

        public static bool HasRectangularSelection(this DataGridContext gridContext)
        {
            foreach (var item in gridContext.GetSelectedItems())
            {
                foreach (var column in gridContext.GetSelectedColumns())
                {
                    var itemIndex = gridContext.Items.IndexOf(item);
                    var columnIndex = gridContext.VisibleColumns.IndexOf(column);

                    var contains = false;

                    foreach (var range in gridContext.SelectedCellRanges)
                    {
                        if (range.ItemRange.Contains(itemIndex) == true && range.ColumnRange.Contains(columnIndex) == true)
                        {
                            contains = true;
                        }
                    }

                    if (contains == false)
                        return false;
                }
            }

            return true;
        }

        public static IEnumerable<SelectionRange> GenerateSelectionRanges(this DataGridContext gridContext, IEnumerable<object> selectedItems)
        {
            var indexList = selectedItems.Select(item => gridContext.Items.IndexOf(item)).OrderBy(item => item).ToList();
            var indexQueue = new Queue<int>(indexList);
            if (indexList.Any())
            {
                var range = new SelectionRange(indexQueue.Dequeue());

                while (indexQueue.Any())
                {
                    var item = indexQueue.Dequeue();

                    if (range.EndIndex == item)
                    {
                        range.EndIndex = item + 1;
                    }
                    else
                    {
                        yield return range;
                        range = new SelectionRange(item);
                    }
                }

                yield return range;
            }
        }

        public static object GetNextItem(this DataGridContext gridContext, object currentItem, bool ignoreChilds)
        {
            var index = gridContext.Items.IndexOf(currentItem);
            var nextIndex = index + 1;

            if (ignoreChilds == false)
            {
                foreach (var detail in gridContext.DetailConfigurations)
                {
                    var childContext = gridContext.GetChildContext(currentItem, detail);
                    if (childContext != null)
                    {
                        if (childContext.Items.Count > 0)
                        {
                            return childContext.Items.GetItemAt(0);
                        }
                    }
                }
            }

            if (gridContext.Items.Count <= nextIndex)
            {
                if (gridContext.ParentDataGridContext != null)
                {
                    return GetNextItem(gridContext.ParentDataGridContext, gridContext.ParentItem, true);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return gridContext.Items.GetItemAt(nextIndex);
            }
        }

        public static object GetPrevItem(this DataGridContext gridContext, object currentItem, bool ignoreChilds)
        {
            var index = gridContext.Items.IndexOf(currentItem);
            var prevIndex = index - 1;

            if (ignoreChilds == false)
            {
                for (var i = gridContext.DetailConfigurations.Count - 1; i >= 0; i--)
                {
                    var detail = gridContext.DetailConfigurations[i];
                    var childContext = gridContext.GetChildContext(currentItem, detail);
                    if (childContext != null)
                    {
                        if (childContext.Items.Count > 0)
                        {
                            return childContext.Items.GetItemAt(childContext.Items.Count - 1);
                        }
                    }
                }
            }

            if (prevIndex < 0)
            {
                if (gridContext.ParentDataGridContext != null)
                {
                    return GetPrevItem(gridContext.ParentDataGridContext, gridContext.ParentItem, true);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return gridContext.Items.GetItemAt(prevIndex);
            }
        }

        public static IEnumerable<object> GetScrollableItems(this DataGridContext gridContext)
        {
            foreach (var item in gridContext.Headers)
            {
                yield return item;
            }

            foreach (var item in gridContext.Items)
            {
                yield return item;

                foreach (var detail in gridContext.DetailConfigurations)
                {
                    var childContext = gridContext.GetChildContext(item, detail);
                    if (childContext != null)
                    {
                        foreach (var i in GetScrollableItems(childContext))
                        {
                            yield return i;
                        }
                    }
                }
            }

            foreach (var item in gridContext.Footers)
            {
                yield return item;
            }
        }

        public static IEnumerable<ModernItemInfo> GetScrollableItemInfos(this DataGridContext gridContext)
        {
            foreach (var item in gridContext.Headers)
            {
                yield return new ModernItemInfo() { GridContext = gridContext, Item = item, };
            }

            foreach (var item in gridContext.Items)
            {
                yield return new ModernItemInfo() { GridContext = gridContext, Item = item, };

                foreach (var detail in gridContext.DetailConfigurations)
                {
                    var childContext = gridContext.GetChildContext(item, detail);
                    if (childContext != null)
                    {
                        foreach (var i in GetScrollableItemInfos(childContext))
                        {
                            yield return i;
                        }
                    }
                }
            }

            foreach (var item in gridContext.Footers)
            {
                yield return new ModernItemInfo() { GridContext = gridContext, Item = item, }; ;
            }
        }
    }
}
