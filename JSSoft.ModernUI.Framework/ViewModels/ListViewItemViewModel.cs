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

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    [Obsolete]
    public abstract class ListViewItemViewModel : PropertyChangedBase, INotifyPropertyChanged, ISelectable, ICommand
    {
        private object itemsViewModel;
        private ICommand defaultCommand;

        private ListViewItemState state = ListViewItemState.IsVisible;
        private bool caseSensitive;
        private string pattern;

        protected ListViewItemViewModel()
        {

        }

        protected ListViewItemViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public virtual string DisplayName => string.Empty;

        public bool IsSelected
        {
            get => this.state.HasFlag(ListViewItemState.IsSelected);
            set
            {
                if (this.state.HasFlag(ListViewItemState.IsSelected) == value)
                    return;

                if (value == true)
                    this.state |= ListViewItemState.IsSelected;
                else
                    this.state &= ~ListViewItemState.IsSelected;

                this.NotifyOfPropertyChange(nameof(this.IsSelected));
            }
        }

        public bool IsVisible
        {
            get => this.state.HasFlag(ListViewItemState.IsVisible);
            set
            {
                if (this.state.HasFlag(ListViewItemState.IsVisible) == value)
                    return;

                if (value == true)
                    this.state |= ListViewItemState.IsVisible;
                else
                    this.state &= ~ListViewItemState.IsVisible;

                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public object Target { get; set; }

        public object ItemsViewModel
        {
            get => this.itemsViewModel;
            set
            {
                this.itemsViewModel = value;
                this.NotifyOfPropertyChange(nameof(this.ItemsViewModel));
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

        public ListViewItemState State
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

        private ICommand DefaultCommand
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
                }
                return this.defaultCommand;
            }
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
