using Caliburn.Micro;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : Screen
    {
        private DataTable table = new DataTable();
        public ShellViewModel()
        {
            this.DisplayName = "Controls";
            this.table.Columns.Add();
            this.table.Columns.Add();
            this.table.Columns.Add();

            this.table.Rows.Add("1", "2", "3");
        }

        public IEnumerable ItemsSource => this.table.DefaultView;
    }
}
