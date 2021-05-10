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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Xceed.Wpf.DataGrid;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public class ModernTextClipboardParser
    {
        private readonly DataGridContext gridContext;
        private readonly string[] columnNames;
        private readonly PropertyDescriptorCollection props;

        public ModernTextClipboardParser(DataGridContext gridContext)
        {
            this.gridContext = gridContext;
            this.Columns = gridContext.Columns.ToArray();

            if (gridContext.Items.SourceCollection is not ITypedList typedList)
            {
                var source = (gridContext.Items.SourceCollection as CollectionView).SourceCollection;
                typedList = source as ITypedList;
            }

            this.props = typedList.GetItemProperties(null);
            this.columnNames = new string[props.Count];
            for (var i = 0; i < props.Count; i++)
            {
                this.columnNames[i] = props[i].Name;
            }
        }

        public static ModernTextClipboardParser Parse(DataGridContext gridContext)
        {
            var parser = new ModernTextClipboardParser(gridContext);
            parser.ParseCore();
            return parser;
        }

        public ColumnBase[] Columns { get; set; }

        public List<string[]> Rows { get; private set; }

        public object[] TargetItems { get; set; }

        public void ParseCore()
        {
            var rows = this.GetLines(Clipboard.GetText());
            var hasHeader = rows.Count > 1 && this.ExistsHeader(rows[0]);

            if (hasHeader == true)
            {
                var titleToColumn = this.Columns.ToDictionary(item => item.Title);
                var columns = new List<ColumnBase>(this.Columns.Length);
                foreach (var item in rows[0])
                {
                    var column = titleToColumn[item];
                    if (column.ReadOnly == true)
                    {
                        throw new Exception(string.Format("'{0}'은(는) 읽기 전용입니다.", column.Title));
                    }
                    columns.Add(column);
                }
                this.Columns = columns.ToArray();
                rows.RemoveAt(0);
            }
            else
            {
                var index = this.gridContext.VisibleColumns.IndexOf(this.gridContext.CurrentColumn);
                if (index + rows[0].Length > this.gridContext.VisibleColumns.Count)
                    throw new Exception("붙여넣기 대상 열의 범위가 초과되었습니다.");
                var columns = new List<ColumnBase>();
                for (var i = 0; i < rows[0].Length; i++)
                {
                    var column = this.gridContext.VisibleColumns[i + index];
                    if (column.ReadOnly == true)
                    {
                        throw new Exception(string.Format("'{0}'은(는) 읽기 전용입니다.", column.Title));
                    }
                    columns.Add(column);
                }
                this.Columns = columns.ToArray();
            }

            this.Rows = rows;
        }

        private bool ExistsHeader(string[] fields)
        {
            var titleToColumn = this.Columns.ToDictionary(item => item.Title);

            foreach (var item in fields)
            {
                if (titleToColumn.ContainsKey(item) == false)
                    return false;
            }

            return true;
        }

        private List<string[]> GetLines(string text)
        {
            var valuesArray = new List<string[]>();
            var lines = text.Split(new string[] { Environment.NewLine, }, StringSplitOptions.None);

            foreach (var item in lines)
            {
                var words = item.Split('\t').Select(w => CorrectionMultiline(w)).ToArray();
                valuesArray.Add(words);
            }

            if (valuesArray.Count > 0)
            {
                var values = valuesArray.Last();
                if (values.Length == 1 && values[0] == string.Empty)
                {
                    valuesArray.RemoveAt(valuesArray.Count - 1);
                }
            }

            var maxColumns = valuesArray.Max(item => item.Length);

            for (int i = 0; i < valuesArray.Count; i++)
            {
                var values = valuesArray[i];

                if (values.Length == maxColumns)
                    continue;

                var rep = Enumerable.Repeat(string.Empty, maxColumns - values.Length);

                valuesArray[i] = values.Concat(rep).ToArray();
            }

            return valuesArray;
        }

        private string CorrectionMultiline(string text)
        {
            if (text.IndexOf('\n') > 0)
            {
                if (text.First() == '\"' && text.Last() == '\"')
                {
                    text = text.Substring(1);
                    text = text.Substring(0, text.Length - 1);
                }
                text = text.Replace("\"\"", "\"");
            }
            // 160 to 32
            return text.Replace('?', ' ');
        }

        public void ValidateRowRange()
        {
            if (this.gridContext.CurrentItemIndex + this.Rows.Count > this.gridContext.Items.Count)
            {
                throw new Exception("붙여넣기 대상 행의 범위가 초과되었습니다.");
            }
        }

        public void SelectRange()
        {
            var columnIndex = this.gridContext.VisibleColumns.IndexOf(this.gridContext.CurrentColumn);
            var itemRange = new SelectionRange(this.gridContext.CurrentItemIndex, this.gridContext.CurrentItemIndex + this.Rows.Count - 1);
            var columnRange = new SelectionRange(columnIndex, columnIndex + this.Columns.Length - 1);
            this.gridContext.SelectedCellRanges.Clear();
            this.gridContext.SelectedCellRanges.Add(new SelectionCellRange(itemRange, columnRange));
        }

        protected object[] CreateFields()
        {
            return new object[columnNames.Length];
        }
    }
}
