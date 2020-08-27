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

using System;
using System.Linq;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework.ViewModels
{
    public abstract class ListBoxItemViewModel : PropertyChangedBase, ISelectable, ICommand
    {
        private ListBoxItemState state = ListBoxItemState.IsVisible;
        private bool caseSensitive;
        private string pattern;
        private EventHandler canExecuteChanged;
        private ICommand command;

        protected ListBoxItemViewModel()
        {

        }

        protected ListBoxItemViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

        }

        public virtual string DisplayName => string.Empty;

        public bool IsSelected
        {
            get => this.state.HasFlag(ListBoxItemState.IsSelected);
            set
            {
                if (this.state.HasFlag(ListBoxItemState.IsSelected) == value)
                    return;

                if (value == true)
                    this.state |= ListBoxItemState.IsSelected;
                else
                    this.state &= ~ListBoxItemState.IsSelected;
                this.command = null;
                this.NotifyOfPropertyChange(nameof(this.IsSelected));
                this.canExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsVisible
        {
            get => this.state.HasFlag(ListBoxItemState.IsVisible);
            set
            {
                if (this.state.HasFlag(ListBoxItemState.IsVisible) == value)
                    return;

                if (value == true)
                    this.state |= ListBoxItemState.IsVisible;
                else
                    this.state &= ~ListBoxItemState.IsVisible;
                this.NotifyOfPropertyChange(nameof(this.IsVisible));
            }
        }

        public object Target { get; set; }

        public object Owner { get; set; }

        public bool CaseSensitive
        {
            get => this.caseSensitive;
            set
            {
                if (this.caseSensitive != value)
                {
                    this.caseSensitive = value;
                    this.NotifyOfPropertyChange(nameof(this.CaseSensitive));
                }
            }
        }

        public string Pattern
        {
            get => this.pattern ?? string.Empty;
            set
            {
                if (this.pattern != value)
                {
                    this.pattern = value;
                    this.NotifyOfPropertyChange(nameof(this.Pattern));
                    this.NotifyOfPropertyChange(nameof(this.HasPattern));
                }
            }
        }

        public bool HasPattern => this.Pattern != string.Empty;

        public ListBoxItemState State
        {
            get => this.state;
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.Refresh();
                }
            }
        }

        protected virtual bool CanExecute(object parameter)
        {
            var query = from item in this.ContextMenus
                        let attr = Attribute.GetCustomAttribute(item.GetType(), typeof(DefaultMenuAttribute), false) as DefaultMenuAttribute
                        where attr != null
                        orderby attr.Order
                        where item.Command != null
                        select item.Command;

            foreach (var item in query)
            {
                if (item.CanExecute(parameter) == true)
                {
                    this.command = item;
                    return true;
                }
            }
            return false;
        }

        protected virtual void Execute(object parameter)
        {
            this.command?.Execute(parameter);
        }

        #region ICommand

        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged += value; }
            remove { this.canExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            this.Execute(parameter);
        }

        #endregion
    }
}
