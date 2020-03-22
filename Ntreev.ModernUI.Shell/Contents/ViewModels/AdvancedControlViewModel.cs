using Ntreev.Library.Random;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class AdvancedControlViewModel : ContentBase, IContent
    {
        private readonly ObservableCollection<string> comboBox = new ObservableCollection<string>();
        private readonly ObservableCollection<string> editableComboBox = new ObservableCollection<string>();
        private readonly ObservableCollection<string> treeViewItem = new ObservableCollection<string>()
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

        private string selectedComboBox;
        private string textBox = "TextBox";
        private Guid guid = Guid.NewGuid();
        private int numericTextBox;
        private string selectedEditableComboBox;
        private string selectedTreeViewItem;

        public AdvancedControlViewModel()
        {
            for (var i = 0; i < RandomUtility.Next(5, 10); i++)
            {
                this.comboBox.Add(RandomUtility.NextWord());
            }
            for (var i = 0; i < RandomUtility.Next(5, 10); i++)
            {
                this.editableComboBox.Add(RandomUtility.NextWord());
            }

            this.SelectedComboBox = this.ComboBox.Random();
            this.SelectedEditableComboBox = this.EditableComboBox.Random();
            this.SelectedTreeViewItem = this.TreeViewItem.Random();
            this.DisplayName = "AdvancedControl";
        }

        public async Task IconButtonAsync()
        {
            await AppMessageBox.ShowAsync("IconButton");
        }

        public IEnumerable<string> ComboBox => this.comboBox;

        public string SelectedComboBox
        {
            get => this.selectedComboBox;
            set
            {
                this.selectedComboBox = value;
                this.NotifyOfPropertyChange(nameof(SelectedComboBox));
            }
        }

        public IEnumerable<string> EditableComboBox => this.editableComboBox;

        public string SelectedEditableComboBox
        {
            get => this.selectedEditableComboBox;
            set
            {
                this.selectedEditableComboBox = value;
                this.NotifyOfPropertyChange(nameof(SelectedEditableComboBox));
            }
        }

        public IEnumerable<string> TreeViewItem => this.treeViewItem;

        public string SelectedTreeViewItem
        {
            get => this.selectedTreeViewItem;
            set
            {
                this.selectedTreeViewItem = value;
                this.NotifyOfPropertyChange(nameof(SelectedTreeViewItem));
            }
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

        public int NumericTextBox
        {
            get => this.numericTextBox;
            set
            {
                this.numericTextBox = value;
                this.NotifyOfPropertyChange(nameof(NumericTextBox));
            }
        }
    }
}
