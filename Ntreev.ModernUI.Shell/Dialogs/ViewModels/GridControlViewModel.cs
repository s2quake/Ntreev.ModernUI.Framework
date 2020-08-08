using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Shell.Dialogs.ViewModels
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
