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
using System.ComponentModel;
using System.Text;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Export;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public class ModernTextClipboardExporter : ClipboardExporterBase
    {
        private readonly StringBuilder sb = new();
        private bool isFistColumn;
        private int indent;

        protected override object ClipboardData => this.sb.ToString();

        protected override void ResetExporter()
        {
            this.sb.Clear();
        }

        protected override void StartExporter(string dataFormat)
        {
            base.StartExporter(dataFormat);
        }

        protected override void StartDataItem(DataGridContext dataGridContext, object dataItem)
        {
            this.isFistColumn = true;
            for (var i = 0; i < this.indent; i++)
            {
                this.sb.Append("\t");
            }
        }

        protected override void StartDataItemField(DataGridContext dataGridContext, Column column, object fieldValue)
        {
            if (this.isFistColumn == true)
            {

            }
            else
            {
                this.sb.Append("\t");
            }


            if (fieldValue is string)
            {
                var text = fieldValue.ToString();
                this.sb.Append($"\"{text}\"");
            }
            else if (fieldValue != null)
            {
                var converter = TypeDescriptor.GetConverter(fieldValue);
                this.sb.Append(converter.ConvertToString(fieldValue));
            }
            this.isFistColumn = false;
        }

        protected override void StartHeader(DataGridContext dataGridContext)
        {
            this.isFistColumn = true;
        }

        protected override void StartHeaderField(DataGridContext dataGridContext, Column column)
        {
            if (this.isFistColumn == true)
            {

            }
            else
            {
                this.sb.Append("\t");
            }

            this.sb.Append(string.Format("\"{0}\"", column.FieldName));
            this.isFistColumn = false;
        }


        protected override void EndDataItem(DataGridContext dataGridContext, object dataItem)
        {
            this.sb.AppendLine();
        }

        protected override void EndDataItemField(DataGridContext dataGridContext, Column column, object fieldValue)
        {

        }

        protected override void EndHeader(DataGridContext dataGridContext)
        {
            this.sb.AppendLine();
        }

        protected override void Indent()
        {
            this.indent++;
        }

        protected override void Unindent()
        {
            this.indent--;
        }
    }
}
