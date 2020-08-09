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

using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    public class TreeViewViewModel : ViewModelBase, ISelector
    {
        private readonly List<TreeViewItemViewModel> expandedItems = new List<TreeViewItemViewModel>();
        private readonly List<TreeViewItemViewModel> visibleItems = new List<TreeViewItemViewModel>();
        private TreeViewItemViewModel selectedItem;
        private string filterExpression;
        private bool caseSensitive;
        private bool globPattern;
        private string displayName;

        public TreeViewViewModel()
        {

        }

        public TreeViewViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public void ExpandAll()
        {
            foreach (var item in this.Items)
            {
                item.ExpandAll();
            }
        }

        public void CollapseAll()
        {
            foreach (var item in this.Items)
            {
                item.CollapseAll();
            }
        }

        public string[] GetSettings()
        {
            var items = EnumerableUtility.FamilyTree(this.Items, item => item.Items)
                                         .Where(item => item.IsExpanded)
                                         .Select(item => this.GetPath(item))
                                         .ToArray();
            return items;
        }

        public async void SetSettings(string[] settings)
        {
            var query = from item in EnumerableUtility.FamilyTree(this.Items, item => item.Items)
                        join setting in settings on this.GetPath(item) equals setting
                        select item;
            var items = query.ToArray();

            foreach (var item in items)
            {
                await this.Dispatcher.InvokeAsync(() =>
                {
                    item.IsExpanded = true;
                }, DispatcherPriority.Background);
            }
        }

        public ObservableCollection<TreeViewItemViewModel> Items { get; } = new ObservableCollection<TreeViewItemViewModel>();

        public TreeViewItemViewModel SelectedItem
        {
            get => this.selectedItem;
            set
            {
                if (this.selectedItem == value)
                    return;

                this.selectedItem = value;

                if (this.selectedItem != null)
                {
                    if (this.selectedItem.IsSelected == false)
                    {
                        this.selectedItem.ExpandAncestors();
                        this.selectedItem.IsSelected = true;
                    }
                }
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public string DisplayName
        {
            get => this.displayName;
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public string FilterExpression
        {
            get => this.filterExpression ?? string.Empty;
            set
            {
                if (this.filterExpression == value)
                    return;

                if (this.FilterExpression == string.Empty)
                {
                    this.BackupState();
                }

                this.filterExpression = value;

                if (this.FilterExpression == string.Empty)
                {
                    this.RestoreState();
                }
                else
                {
                    var items = this.Items.SelectMany((TreeViewItemViewModel item) => EnumerableUtility.FamilyTree(item, i => i.Items));

                    foreach (var item in items)
                    {
                        item.IsVisible = false;
                        item.IsExpanded = false;
                    }

                    var query = from item in items
                                where this.Filter(item.DisplayName, this.FilterExpression)
                                select item;

                    foreach (var item in query)
                    {
                        item.Pattern = this.FilterExpression;
                        item.CaseSensitive = this.CaseSensitive;
                        item.Show();
                    }

                    var singleItem = query.FirstOrDefault();
                    if (singleItem != null)
                        singleItem.IsSelected = true;
                }

                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
            }
        }

        public bool CaseSensitive
        {
            get => this.caseSensitive;
            set
            {
                this.caseSensitive = value;
                this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
            }
        }

        public bool GlobPattern
        {
            get => this.globPattern;
            set
            {
                this.globPattern = value;
                this.NotifyOfPropertyChange(nameof(this.GlobPattern));
            }
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        private bool Filter(string text, string filterExpression)
        {
            if (this.GlobPattern == true)
                return StringUtility.Glob(text, filterExpression, this.CaseSensitive);
            else if (this.CaseSensitive == false)
                return text.IndexOf(filterExpression, StringComparison.OrdinalIgnoreCase) >= 0;
            return text.IndexOf(filterExpression) >= 0;
        }

        private void BackupState()
        {
            var items = this.Items.SelectMany((TreeViewItemViewModel item) => EnumerableUtility.FamilyTree(item, i => i.Items));

            this.expandedItems.Clear();
            this.visibleItems.Clear();
            foreach (var item in items)
            {
                if (item.IsExpanded == true)
                    this.expandedItems.Add(item);
                if (item.IsVisible == true)
                    this.visibleItems.Add(item);
            }
        }

        private void RestoreState()
        {
            var selectedItem = this.selectedItem;
            var items = this.Items.SelectMany((TreeViewItemViewModel item) => EnumerableUtility.FamilyTree(item, i => i.Items));

            foreach (var item in items)
            {
                item.IsVisible = false;
                item.IsExpanded = false;
                item.Pattern = string.Empty;
            }

            foreach (var item in this.expandedItems)
            {
                item.IsExpanded = true;
            }

            foreach (var item in this.visibleItems)
            {
                item.IsVisible = true;
            }

            if (selectedItem != null)
            {
                selectedItem.Show();
                selectedItem.IsSelected = true;
            }
        }

        private string GetPath(TreeViewItemViewModel viewModel)
        {
            var ancestors = EnumerableUtility.Ancestors(viewModel, item => item.Parent);

            var items = EnumerableUtility.Friends(viewModel, ancestors)
                                         .Reverse()
                                         .Select(item => item.DisplayName.EscapeChar(PathUtility.SeparatorChar));

            return string.Join(PathUtility.Separator, items);
        }

        #region ISelector

        object ISelector.SelectedItem
        {
            get => this.SelectedItem;
            set => this.SelectedItem = value as TreeViewItemViewModel;
        }

        #endregion
    }
}
