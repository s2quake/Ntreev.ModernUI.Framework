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

using Caliburn.Micro;
using Ntreev.Library;
using Ntreev.Library.IO;
using Ntreev.Library.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    public class ListBoxViewModel<T> : ViewModelBase, ISelector, IPartImportsSatisfiedNotification where T : ListBoxItemViewModel
    {
        private readonly ObservableCollection<T> items = new ObservableCollection<T>();
        private readonly List<T> visibleItems = new List<T>();
        private T selectedItem;
        private string filterExpression;
        private bool caseSensitive;
        private bool globPattern;
        private string displayName;

        [Import]
        private IServiceProvider serviceProvider = null;
        [Import]
        private ICompositionService compositionService = null;

        public ListBoxViewModel()
        {
            this.items.CollectionChanged += Items_CollectionChanged;
        }
        
        [Obsolete]
        public ObservableCollection<T> ItemsSource
        {
            get { return this.items; }
        }

        public ObservableCollection<T> Items
        {
            get { return this.items; }
        }

        public T SelectedItem
        {
            get { return this.selectedItem; }
            set
            {
                if (value != null && this.items.Contains(value) == false)
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
            get { return this.displayName; }
            set
            {
                this.displayName = value;
                this.NotifyOfPropertyChange(nameof(this.DisplayName));
            }
        }

        public string FilterExpression
        {
            get { return this.filterExpression ?? string.Empty; }
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
                    foreach (var item in this.items)
                    {
                        item.IsVisible = false;
                    }

                    var query = from item in items
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
            get { return this.caseSensitive; }
            set
            {
                this.caseSensitive = value;
                this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
            }
        }

        public bool GlobPattern
        {
            get { return this.globPattern; }
            set
            {
                this.globPattern = value;
                this.NotifyOfPropertyChange(nameof(this.GlobPattern));
            }
        }

        public event EventHandler SelectionChanged;

        public virtual IEnumerable<IToolBarItem> ToolBarItems
        {
            get
            {
                if (this.serviceProvider == null)
                    return Enumerable.Empty<IToolBarItem>();
                return ToolBarItemUtility.GetToolBarItems(this, this.serviceProvider);
            }
        }

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            this.SelectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPartImportsSatisfied()
        {

        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (var item in e.NewItems)
                        {
                            this.compositionService?.SatisfyImportsOnce(item);
                        }
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
            foreach (var item in this.items)
            {
                if (item.IsVisible == true)
                    this.visibleItems.Add(item);
            }
        }

        private void RestoreState()
        {
            var selectedItem = this.selectedItem;

            foreach (var item in this.items)
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

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            foreach (var item in this.Items)
            {
                this.compositionService.SatisfyImportsOnce(item);
            }
            this.OnPartImportsSatisfied();
        }

        #endregion

        #region ISelector

        object ISelector.SelectedItem
        {
            get
            {
                return this.SelectedItem;
            }
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
