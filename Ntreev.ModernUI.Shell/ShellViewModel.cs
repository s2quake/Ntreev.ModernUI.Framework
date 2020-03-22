using Ntreev.ModernUI.Framework;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Input;

namespace Ntreev.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : ScreenBase
    {
        private readonly DataTable table = new DataTable();

        private readonly ICommand insertCommand;
        private string textBox = "TextBox";
        private Guid guid = Guid.NewGuid();

        public ShellViewModel()
        {
            this.DisplayName = "Controls";
            this.table.Columns.Add();
            this.table.Columns.Add();
            this.table.Columns.Add();

            this.table.Rows.Add("1", "Value1", "3");
            this.table.Rows.Add("2", "Value2", "3");

            this.insertCommand = new DelegateCommand((p) => this.Insert(), (p) => CanInsert);
        }

        public void Insert()
        {

        }

        public string TextBox
        {
            get => this.textBox;
            set
            {
                this.textBox = value;
                this.NotifyOfPropertyChange(nameof(TextBox));
            }
        }

        public Guid Guid
        {
            get => this.guid;
            set
            {
                this.guid = value;
                this.NotifyOfPropertyChange(nameof(Guid));
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
