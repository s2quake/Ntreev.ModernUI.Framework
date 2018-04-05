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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.Linq.Expressions;
using Caliburn.Micro;
using System.Collections;
using Ntreev.Library.Linq;
using System.ComponentModel.Composition;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.ViewModels
{
    public abstract class ListBoxItemViewModel : PropertyChangedBase, ISelectable, ICommand, IPartImportsSatisfiedNotification
    {
        private ICommand defaultCommand;

        private ListBoxItemState state = ListBoxItemState.IsVisible;
        private bool caseSensitive;
        private string pattern;

        [Import]
        private IServiceProvider serviceProvider = null;

        protected ListBoxItemViewModel()
        {

        }

        public virtual string DisplayName
        {
            get { return null; }
        }

        public bool IsSelected
        {
            get { return this.state.HasFlag(ListBoxItemState.IsSelected); }
            set
            {
                if (this.state.HasFlag(ListBoxItemState.IsSelected) == value)
                    return;

                if (value == true)
                    this.state |= ListBoxItemState.IsSelected;
                else
                    this.state &= ~ListBoxItemState.IsSelected;

                this.NotifyOfPropertyChange(nameof(this.IsSelected));
            }
        }

        public bool IsVisible
        {
            get { return this.state.HasFlag(ListBoxItemState.IsVisible); }
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
            get { return this.caseSensitive; }
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
            get { return this.pattern ?? string.Empty; }
            set
            {
                if (this.pattern == value)
                    return;
                this.pattern = value;
                this.NotifyOfPropertyChange(nameof(this.Pattern));
                this.NotifyOfPropertyChange(nameof(this.HasPattern));
            }
        }

        public bool HasPattern
        {
            get { return this.Pattern != string.Empty; }
        }

        public ListBoxItemState State
        {
            get { return this.state; }
            set
            {
                if (this.state == value)
                    return;
                this.state = value;
                this.Refresh();
            }
        }

        public virtual IEnumerable<IMenuItem> ContextMenus
        {
            get
            {
                if (this.serviceProvider == null)
                    return Enumerable.Empty<IMenuItem>();
                return MenuItemUtility.GetMenuItems<IMenuItem>(this, this.serviceProvider);
            }
        }

        protected virtual void OnImportsSatisfied()
        {

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

        #region IPartImportsSatisfiedNotification

        void IPartImportsSatisfiedNotification.OnImportsSatisfied()
        {
            this.OnImportsSatisfied();
        }

        #endregion

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
