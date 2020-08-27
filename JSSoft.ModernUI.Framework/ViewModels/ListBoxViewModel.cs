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

using JSSoft.Library;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace JSSoft.ModernUI.Framework.ViewModels
{
    public class ListBoxViewModel<T> : ViewModelBase, ISelector where T : ListBoxItemViewModel
    {
        private readonly List<T> visibleItems = new List<T>();
        private T selectedItem;
        private string filterExpression;
        private bool caseSensitive;
        private bool globPattern;
        private string displayName;

        public ListBoxViewModel()
            : this(null)
        {

        }

        public ListBoxViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Items.CollectionChanged += Items_CollectionChanged;
        }

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public T SelectedItem
        {
            get => this.selectedItem;
            set
            {
                if (value != null && this.Items.Contains(value) == false)
                    throw new ArgumentOutOfRangeException(nameof(value));
                if (this.selectedItem == value)
                    return;

                if (this.selectedItem != null)
                {
                    this.selectedItem.IsSelected = false;
                }

                this.selectedItem = value;

                if (this.selectedItem != null)
                {
                    this.selectedItem.IsSelected = true;
                }
                this.NotifyOfPropertyChange(nameof(this.SelectedItem));
                this.OnSelectionChanged(EventArgs.Empty);
            }
        }

        public string DisplayName
        {
            get => this.displayName ?? string.Empty;
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
                    foreach (var item in this.Items)
                    {
                        item.IsVisible = false;
                    }

                    var query = from item in Items
                                where this.Filter(item.DisplayName, this.FilterExpression)
                                select item;

                    foreach (var item in query)
                    {
                        item.Pattern = this.FilterExpression;
                        item.CaseSensitive = this.CaseSensitive;
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

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (var item in e.OldItems)
                        {
                            if (this.SelectedItem == item)
                            {
                                this.SelectedItem = null;
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    {
                        this.SelectedItem = null;
                    }
                    break;
            }
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
            this.visibleItems.Clear();
            foreach (var item in this.Items)
            {
                if (item.IsVisible == true)
                    this.visibleItems.Add(item);
            }
        }

        private void RestoreState()
        {
            var selectedItem = this.selectedItem;

            foreach (var item in this.Items)
            {
                item.IsVisible = false;
                item.Pattern = string.Empty;
            }

            foreach (var item in this.visibleItems)
            {
                item.IsVisible = true;
            }

            if (selectedItem != null)
            {
                selectedItem.IsSelected = true;
            }
        }

        #region ISelector

        object ISelector.SelectedItem
        {
            get => this.SelectedItem;
            set
            {
                if (value is T viewModel)
                    this.SelectedItem = viewModel;
                else
                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
