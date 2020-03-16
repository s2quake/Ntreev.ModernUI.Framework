//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_TreeView, Type = typeof(TreeView))]
    public class TreeViewItemSelector : ComboBox
    {
        public const string PART_TreeView = nameof(PART_TreeView);

        private Dictionary<object, TreeViewItemViewModel> itemToViewModel;
        private TreeView treeView;

        public TreeViewItemSelector()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.treeView = this.Template.FindName(PART_TreeView, this) as TreeView;

            if (this.treeView != null)
            {
                this.UpdateItemsSource();

                this.treeView.PreviewMouseDoubleClick += TreeView_PreviewMouseDoubleClick;
                this.treeView.KeyDown += TreeView_KeyDown;
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (this.treeView != null)
            {
                this.UpdateItemsSource();
            }
        }

        protected async override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
            if (this.treeView != null)
            {
                if (this.SelectedItem != null && this.itemToViewModel.ContainsKey(this.SelectedItem) == true)
                {
                    var viewModel = this.itemToViewModel[this.SelectedItem];
                    viewModel.IsSelected = true;
                    await this.Dispatcher.InvokeAsync(() =>
                    {
                        this.treeView.Focus();
                    });
                }
            }
        }

        private void TreeView_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.None)
                            this.SelectedItem = this.treeView.SelectedValue;
                    }
                    break;
            }
        }

        private void TreeView_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.IsDropDownOpen = false;
                this.SelectedItem = this.treeView.SelectedValue;
                e.Handled = true;
            }
        }

        private void UpdateItemsSource()
        {
            if (this.ItemsSource != null)
            {
                if (this.ItemsSource is IEnumerable<string> == true)
                {
                    var items = this.ItemsSource as IEnumerable<string>;
                    var viewModel = CategoryTreeViewItemViewModel.FromItems(items.ToArray());
                    var viewModels = EnumerableUtility.FamilyTree<TreeViewItemViewModel>(viewModel, item => item.Items);
                    foreach (var item in viewModels.Where(i => i is CategoryTreeViewItemViewModel))
                    {
                        item.IsExpanded = true;
                    }
                    this.treeView.ItemsSource = new CategoryTreeViewItemViewModel[] { viewModel, };
                    this.treeView.SelectedValuePath = nameof(CategoryTreeViewItemViewModel.Path);
                    this.itemToViewModel = viewModels.ToDictionary(item => item.Target);
                }
                else if (this.ItemsSource is IEnumerable<TreeViewItemViewModel> == true)
                {
                    var items = this.ItemsSource as IEnumerable<TreeViewItemViewModel>;
                    this.treeView.ItemsSource = items.Where(item => item.Parent == null);
                    this.itemToViewModel = items.ToDictionary(item => (object)item);
                    this.treeView.SelectedValuePath = string.Empty;
                }
                else
                {
                    var items = new List<TreeViewItemViewModel>();
                    foreach (var item in this.ItemsSource)
                    {
                        items.Add(new InternalTreeViewItemViewModel(item));
                    }
                    this.itemToViewModel = items.ToDictionary(item => item.Target);
                    this.treeView.SelectedValuePath = nameof(InternalTreeViewItemViewModel.Path);
                }
            }
            else
            {
                this.treeView.ItemsSource = null;
                this.itemToViewModel = new Dictionary<object, TreeViewItemViewModel>();
            }
        }

        #region classes

        class InternalTreeViewItemViewModel : TreeViewItemViewModel
        {
            private readonly object item;

            public InternalTreeViewItemViewModel(object item)
            {
                this.item = item;
                this.Target = item;
            }

            public string Path
            {
                get { return TreeViewItemViewModel.BuildPath(this); }
            }

            public override string DisplayName
            {
                get
                {
                    if (this.item == null)
                        return string.Empty;
                    return this.item.ToString();
                }
            }
        }

        #endregion
    }
}
