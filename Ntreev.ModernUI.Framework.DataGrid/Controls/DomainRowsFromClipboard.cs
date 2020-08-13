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
using System.Windows;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    class DomainRowsFromClipboard
    {
        private readonly DataGridContext gridContext;
        private readonly List<string> keys = new List<string>();
        private ColumnBase[] columns;

        public DomainRowsFromClipboard(DataGridContext gridContext)
        {
            this.gridContext = gridContext;
            this.columns = gridContext.Columns.ToArray();
        }

        public List<string[]> GetFieldsList()
        {
            var fieldsList = this.GetLines(Clipboard.GetText());

            bool hasHeader = fieldsList.Count > 1 && this.ExistsHeader(fieldsList[0]);

            if (hasHeader == true)
            {
                var titleToColumn = this.columns.ToDictionary(item => item.Title);
                var columns = new List<ColumnBase>(this.columns.Length);
                foreach (var item in fieldsList[0])
                {
                    columns.Add(titleToColumn[item]);
                }
                this.columns = columns.ToArray();
                fieldsList.RemoveAt(0);
            }
            else
            {


            }

            return fieldsList;
        }

        private bool ExistsHeader(string[] fields)
        {
            var titleToColumn = this.columns.ToDictionary(item => item.Title);

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

            for (var i = 0; i < valuesArray.Count; i++)
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
            return text.Replace(' ', ' ');
        }
    }
}