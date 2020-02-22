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
using System.Windows.Input;

namespace Ntreev.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : Screen
    {
        private DataTable table = new DataTable();

        private readonly ICommand insertCommand;
        private string text = "test";

        public ShellViewModel()
        {
            this.DisplayName = "Controls";
            this.table.Columns.Add();
            this.table.Columns.Add();
            this.table.Columns.Add();

            this.table.Rows.Add("1", "Value1", "3");
            this.table.Rows.Add("2", "Value2", "3");

            var ss = Guid.NewGuid();
            this.insertCommand = new DelegateCommand((p) => this.Insert(), (p) => CanInsert);
        }

        public void Insert()
        {

        }

        public string Text
        {
            get => this.text;
            set
            {
                this.text = value;
                throw new Exception("!23");
                this.NotifyOfPropertyChange(nameof(Text));
            }
        }

        public bool CanInsert => true;

        public ICommand InsertCommand => this.insertCommand;

        public IEnumerable ItemsSource => this.table.DefaultView;

        public string[] TreeViewItems { get; } = new string[]
        {
            "/",
            "/Root/",
            "/Root/Types/",
            "/Root/Types/Type1",
            "/Root/Types/Type2",
            "/Root/Tables/",
            "/Root/Tables/Table1",
            "/Root/Tables/Table2",
        };
    }
}
