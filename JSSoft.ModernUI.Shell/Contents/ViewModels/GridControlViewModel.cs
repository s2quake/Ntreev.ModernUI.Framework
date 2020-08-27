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
