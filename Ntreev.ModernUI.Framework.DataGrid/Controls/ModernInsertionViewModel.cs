using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    class ModernInsertionViewModel : ModalDialogBase
    {
        List<object> itemsSource = new List<object>();
        public ModernInsertionViewModel()
        {
            this.itemsSource.Add(true);
            this.itemsSource.Add(1);
        }

        public IEnumerable ItemsSource
        {
            get { return this.itemsSource; }
        }
    }
}
