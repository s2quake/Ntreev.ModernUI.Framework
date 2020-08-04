using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Shell.Dialogs.ViewModels
{
    class GridControlViewModel : ModalDialogBase
    {
        private readonly DataTable table = new DataTable();

        public async Task OKAsync()
        {
            await this.TryCloseAsync();
        }

        public IEnumerable ItemsSource => this.table.DefaultView;
    }
}
