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
using Ntreev.Library.ObjectModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    public abstract class TreeViewItemViewModel : PropertyChangedBase, INotifyPropertyChanged, IComparable, IDisposable, ISelectable, ICommand
    {
        private TreeViewItemViewModel parent;
        private ICommand defaultCommand;

        private TreeViewItemState state = TreeViewItemState.IsVisible;
        private bool caseSensitive;
        private string pattern;

        protected TreeViewItemViewModel()
        {
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.ExpandCommand = new DelegateCommand(item =>
            {
                this.IsExpanded = !this.IsExpanded;
            });
        }

        protected TreeViewItemViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.Items.CollectionChanged += Items_CollectionChanged;
            this.ExpandCommand = new DelegateCommand(item =>
            {
                this.IsExpanded = !this.IsExpanded;
            });
        }

        public int Depth
        {
            get
            {
                if (this.parent == null)
                    return 0;
                return this.parent.Depth + 1;
            }
        }

        public virtual int Order => 0;

        public virtual int CompareTo(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (object.ReferenceEquals(this, obj) == true)
                return 0;

            var vm = obj as TreeViewItemViewModel;
            if (vm.Order != this.Order)
            {
                if (vm.Order > this.Order)
                    return 1;
                return -1;
            }
            return (this.DisplayName ?? string.Empty).CompareTo(vm.DisplayName);
        }

        public void Dispose()
        {
            this.Parent = null;
            this.OnDisposed(EventArgs.Empty);
        }

        public void ExpandAncestors()
        {
            foreach (var item in EnumerableUtility.Ancestors(this, i => i.Parent))
            {
                if (item == null)
                    continue;
                item.IsExpanded = true;
            }
        }

        public void ExpandAll()
        {
            if (this.HasItems == true)
            {
                this.IsExpanded = true;
                foreach (var item in this.Items)
                {
                    item.ExpandAll();
                }
            }
        }

        public void CollapseAll()
        {
            if (this.HasItems == true)
            {
                this.IsExpanded = false;
                foreach (var item in this.Items)
                {
                    item.CollapseAll();
                }
            }
        }

        public void Show()
        {
            this.IsVisible = true;
            var items = EnumerableUtility.Ancestors(this, item => item.Parent);
            foreach (var item in items)
            {
                item.IsVisible = true;
                item.IsExpanded = true;
            }
        }

        public void Filter(string pattern, Func<bool, TreeViewItemViewModel> predicate)
        {

        }

        public static TreeViewItemViewModel GetRoot(TreeViewItemViewModel viewModel)
        {
            if (viewModel.Parent == null)
                return viewModel;
            return EnumerableUtility.Ancestors(viewModel, item => item.Parent).First();
        }

        public static IEnumerable<TreeViewItemViewModel> FamilyTree(TreeViewItemViewModel viewModel)
        {
            return EnumerableUtility.FamilyTree<TreeViewItemViewModel>(viewModel, item => item.Items);
        }

        public static IEnumerable<TreeViewItemViewModel> Descendants(TreeViewItemViewModel viewModel)
        {
            return EnumerableUtility.Descendants<TreeViewItemViewModel>(viewModel, item => item.Items);
        }

        public static string BuildPath(TreeViewItemViewModel viewModel)
        {
            var items = CollectDisplayName(null, viewModel).Reverse();
            return CategoryName.Create(items.ToArray());
        }

        public static string BuildRelativePath(TreeViewItemViewModel parent, TreeViewItemViewModel viewModel)
        {
            return BuildRelativePath(parent, viewModel, item => true);
        }

        public static string BuildRelativePath(TreeViewItemViewModel parent, TreeViewItemViewModel viewModel, Predicate<TreeViewItemViewModel> isCategory)
        {
            var items = CollectDisplayName(parent, viewModel).Reverse();
            if (isCategory(viewModel) == true)
                return CategoryName.Create(items.ToArray());
            return ItemName.Create(items.ToArray());
        }

        public bool HasItems => this.Items.Count > 0;

        public virtual string DisplayName => string.Empty;

        public TreeViewItemCollection Items { get; } = new TreeViewItemCollection();

        public bool IsExpanded
        {
            get => this.state.HasFlag(TreeViewItemState.IsExpanded);
            set
            {
                if (this.state.HasFlag(TreeViewItemState.IsExpanded) == value)
                    return;

                if (value == true)
                    this.state |= TreeViewItemState.IsExpanded;
                else
                    this.state &= ~TreeViewItemState.IsExpanded;
                this.NotifyOfPropertyChange(nameof(this.IsExpanded));

                if (this.state.HasFlag(TreeViewItemState.IsExpanded) == true && this.parent != null)
                {
                    this.parent.IsExpanded = true;
                }
            }
        }

        public bool IsSelected
        {
            get => this.state.HasFlag(TreeViewItemState.IsSelected);
            set
            {
                if (this.state.HasFlag(TreeViewItemState.IsSelected) == value)
                    return;

                if (value == true)
                    this.state |= TreeViewItemState.IsSelected;
                else
                    this.state &= ~TreeViewItemState.IsSelected;

                this.NotifyOfPropertyChange(nameof(this.IsSelected));
            }
        }

        public bool IsVisible
        {
            get => this.state.HasFlag(TreeViewItemState.IsVisible);
            set
            {
                if (this.state.HasFlag(TreeViewItemState.IsVisible) == value)
                    return;

                if (value == true)
                    this.state |= TreeViewItemState.IsVisible;
                else
                    this.state &= ~TreeViewItemState.IsVisible;

                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public bool IsParentExpanded
        {
            get
            {
                if (this.parent == null)
                    return true;
                if (this.parent.IsExpanded == true)
                    return true;
                return false;
            }
        }

        public TreeViewItemViewModel Parent
        {
            get => this.parent;
            set
            {
                if (this.parent == value)
                    return;

                if (this.parent != null)
                {
                    this.parent.Items.Remove(this);
                }
                this.parent = value;
                if (this.parent != null)
                {
                    var collection = this.parent.Items;
                    var insert = -1;
                    for (var i = 0; i < collection.Count; i++)
                    {
                        var item = collection[i];
                        if (item.CompareTo(this) > 0)
                        {
                            insert = i;
                            break;
                        }
                    }
                    if (insert == -1)
                        collection.Add(this);
                    else
                        collection.Insert(insert, this);
                }
            }
        }

        public object Target { get; set; }

        public object Owner { get; set; }

        public bool IsDebug
        {
            get
            {
#if DEBUG
                return true;
#else
                return false;
#endif
            }
        }

        public bool CaseSensitive
        {
            get => this.caseSensitive;
            set
            {
                if (this.caseSensitive == value)
                    return;
                this.caseSensitive = value;
                this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
            }
        }

        public string Pattern
        {
            get => this.pattern ?? string.Empty;
            set
            {
                if (this.pattern == value)
                    return;
                this.pattern = value;
                this.NotifyOfPropertyChange(nameof(this.Pattern));
                this.NotifyOfPropertyChange(nameof(this.HasPattern));
            }
        }

        public bool HasPattern => this.Pattern != string.Empty;

        public TreeViewItemState State
        {
            get => this.state;
            set
            {
                if (this.state == value)
                    return;
                this.state = value;
                this.Refresh();
            }
        }

        public ICommand ExpandCommand { get; }

        public virtual ICommand DefaultCommand
        {
            get
            {
                if (this.defaultCommand == null)
                {
                    var query = from item in this.ContextMenus
                                let attr = Attribute.GetCustomAttribute(item.GetType(), typeof(DefaultMenuAttribute), false) as DefaultMenuAttribute
                                where attr != null
                                orderby attr.Order
                                select item;

                    if (query.Any() == true)
                    {
                        this.defaultCommand = query.First().Command;
                    }
                    else
                    {
                        this.defaultCommand = this.ExpandCommand;
                    }
                }

                return this.defaultCommand;
            }
        }

        public event EventHandler Disposed;

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        private static IEnumerable<string> CollectDisplayName(TreeViewItemViewModel parent, TreeViewItemViewModel viewModel)
        {
            if (viewModel != parent)
            {
                yield return viewModel.DisplayName;
                foreach (var item in CollectDisplayName(parent, viewModel.Parent))
                {
                    yield return item;
                }
            }
        }

        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        foreach (TreeViewItemViewModel item in e.NewItems)
                        {
                            item.parent = this;
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        foreach (TreeViewItemViewModel item in e.OldItems)
                        {
                            item.parent = null;
                        }
                    }
                    break;
            }
            this.NotifyOfPropertyChange(nameof(this.HasItems));
        }

        #region ICommand

        event EventHandler ICommand.CanExecuteChanged
        {
            add
            {
                if (this.DefaultCommand != null)
                    this.DefaultCommand.CanExecuteChanged += value;
            }
            remove
            {
                if (this.DefaultCommand != null)
                    this.DefaultCommand.CanExecuteChanged -= value;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (this.DefaultCommand != null)
                return this.DefaultCommand.CanExecute(parameter);
            return false;
        }

        void ICommand.Execute(object parameter)
        {
            if (this.DefaultCommand != null)
                this.DefaultCommand.Execute(parameter);
        }

        #endregion
    }
}
