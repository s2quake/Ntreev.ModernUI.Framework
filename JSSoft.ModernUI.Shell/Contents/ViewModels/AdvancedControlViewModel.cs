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

using JSSoft.Library.Random;
using JSSoft.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class AdvancedControlViewModel : ContentBase, IContent
    {
        private readonly ObservableCollection<string> comboBox = new();
        private readonly ObservableCollection<string> editableComboBox = new();
        private readonly ObservableCollection<string> treeViewItem = new()
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

        [ImportingConstructor]
        public AdvancedControlViewModel(IShell shell)
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
