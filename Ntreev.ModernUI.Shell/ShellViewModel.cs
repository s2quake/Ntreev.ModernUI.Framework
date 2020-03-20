using Caliburn.Micro;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;
using System;
using System.Collections;
using System.ComponentModel.Composition;
using System.Data;
using System.Windows.Input;

namespace Ntreev.ModernUI.Shell
{
    [Export(typeof(IShell))]
    class ShellViewModel : Screen
    {
        private DataTable table = new DataTable();

        private readonly ICommand insertCommand;
        private string text = "test";
        private Guid guid = Guid.NewGuid();

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

            //var sss = new int[2, 3] { { 1, 2, 3 }, { 4, 5, 6 } };
            //var sourceValue = sss as Array;
            //var lengthList = new List<int>(sourceValue.Rank);
            //for (var i = 0; i < sourceValue.Rank; i++)
            //{
            //    lengthList.Add(sourceValue.GetLength(i));
            //}
            //ArrayList arrayList = new ArrayList(sourceValue.Length);
            //foreach(var item in sourceValue)
            //{
            //    arrayList.Add((decimal)(int)item);
            //}


            //int[] indics = new int[lengthList.Count];
            //var ddd = Array.CreateInstance(typeof(decimal), lengthList.ToArray());
            //while (IncrementIndics(indics, lengthList.ToArray()))
            //{
            //    var v = sourceValue.GetValue(indics);
            //    ddd.SetValue((decimal)(int)v, indics);
            //}



            //arrayList.CopyTo(ddd);



            //var s = sss as IList;
            //var a= s[5];
        }

        public async void PickColor()
        {
            var dialog = new SelectColorViewModel();
            if (await dialog.ShowDialogAsync() == true)
            {

            }
        }

        public static bool IncrementIndics(int[] indics, int[] length)
        {
            indics[length.Length - 1]++;
            for (var i = indics.Length - 1; i >= 0; i--)
            {
                if (indics[i] >= length[i])
                {
                    indics[i] = 0;
                    if (i == 0)
                        return false;
                    indics[i - 1]++;
                }
            }
            return true;
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
