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
using System.Collections;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Input;

namespace JSSoft.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class GridControlViewModel : ContentBase, IContent
    {
        private readonly DataTable table = new DataTable();

        public GridControlViewModel()
        {
            this.DisplayName = "GridControl";

            this.table.Columns.Add();
            this.table.Columns.Add();
            this.table.Columns.Add();

            this.table.Rows.Add("1", "Value1", "3");
            this.table.Rows.Add("2", "Value2", "3");

            this.InsertCommand = new DelegateCommand((p) => this.Insert(), (p) => CanInsert);
        }

        public void Insert()
        {

        }

        public bool CanInsert => true;

        public ICommand InsertCommand { get; private set; }

        public IEnumerable ItemsSource => this.table.DefaultView;
    }
}
