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
using JSSoft.ModernUI.Framework;
using System;
using System.Collections;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Shell.Dialogs.ViewModels
{
    class GridControlViewModel : ModalDialogBase
    {
        private readonly DataTable table = new DataTable();

        public GridControlViewModel()
        {
            this.table.Columns.Add("Color");
            this.table.Columns.Add("LockInfo");
            this.table.Columns.Add("AccessInfo");
            this.table.Columns.Add("IsLoaded", typeof(bool));
            this.table.Columns.Add("Name");
            this.table.Columns.Add("Revision", typeof(int));
            this.table.Columns.Add("Comment");
            this.table.Columns.Add("CreationID");
            this.table.Columns.Add("CreationDateTime", typeof(DateTime));
            this.table.Columns.Add("ModificationID");
            this.table.Columns.Add("ModificationDateTime", typeof(DateTime));
        }

        public async Task OKAsync()
        {
            await this.TryCloseAsync(true);
        }

        public IEnumerable ItemsSource => this.table.DefaultView;

        protected override async Task OnInitializeAsync(CancellationToken cancellationToken)
        {
            await base.OnInitializeAsync(cancellationToken);

            var row = this.table.NewRow();
            row["Color"] = "Red";
            row["LockInfo"] = "Locked";
            row["AccessInfo"] = "Private";
            row["IsLoaded"] = true;
            row["Name"] = "master";
            row["Revision"] = 1000;
            row["Comment"] = "";
            row["CreationID"] = "s2quake";
            row["CreationDateTime"] = DateTime.Now;
            row["ModificationID"] = "s2quake";
            row["ModificationDateTime"] = DateTime.Now;
            this.table.Rows.Add(row);
        }
    }
}
