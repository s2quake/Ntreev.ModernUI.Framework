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

using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ntreev.ModernUI.Framework
{
    public abstract class MenuItemBase : PropertyChangedBase, IMenuItem, ICommand
    {
        private bool isChecked;
        private string displayName;
        private EventHandler canExecuteChanged;
        private object icon;

        protected MenuItemBase()
        {

        }

        protected MenuItemBase(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {

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

        public ICommand Command => this;

        public InputGesture InputGesture { get; set; }

        public virtual IEnumerable<IMenuItem> MenuItems
        {
            get
            {
                if (this.ServiceProvider != null)
                    return MenuItemUtility.GetMenuItems(this, this.ServiceProvider);
                return Enumerable.Empty<IMenuItem>();
            }
        }

        public bool IsVisible
        {
            get
            {
                if (this.HideOnDisabled == true && this.IsEnabled == false)
                    return false;
                return true;
            }
        }

        public bool IsChecked
        {
            get => this.isChecked;
            set
            {
                this.isChecked = value;
                this.NotifyOfPropertyChange(nameof(this.IsChecked));
            }
        }

        public bool IsEnabled { get; private set; }

        public object Icon
        {
            get => this.icon;
            set
            {
                if (value is string uri)
                {
                    if (uri.StartsWith("pack://application:,,,") == false)
                    {
                        if (uri.StartsWith("/") == true)
                            uri = "pack://application:,,," + uri;
                        else
                            uri = "pack://application:,,,/" + uri;
                    }
                    this.icon = new IconImage() { Source = new BitmapImage(new Uri(uri)) };
                }
                else
                {
                    this.icon = value;
                }

                this.NotifyOfPropertyChange(nameof(this.Icon));
            }
        }

        public bool HideOnDisabled { get; set; }

        protected virtual void OnExecute(object parameter)
        {

        }

        protected virtual bool OnCanExecute(object parameter)
        {
            return true;
        }

        protected void InvokeCanExecuteChangedEvent()
        {
            this.canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void InvokeCanExecuteChangedEvent(object sender, EventArgs e)
        {
            this.canExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        #region ICommand

        event EventHandler ICommand.CanExecuteChanged
        {
            add { this.canExecuteChanged += value; }
            remove { this.canExecuteChanged -= value; }
        }

        bool ICommand.CanExecute(object parameter)
        {
            try
            {
                var oldValue = this.IsEnabled;
                var newValue = this.OnCanExecute(parameter);
                if (oldValue != newValue)
                {
                    this.IsEnabled = newValue;
                }
                return this.IsEnabled;
            }
            finally
            {
                this.Dispatcher.InvokeAsync(() =>
                {
                    this.NotifyOfPropertyChange(nameof(this.IsEnabled));
                    this.NotifyOfPropertyChange(nameof(this.IsVisible));
                });
            }
        }

        void ICommand.Execute(object parameter)
        {
            this.OnExecute(parameter);
        }

        #endregion
    }

    public abstract class MenuItemBase<T> : MenuItemBase
    {
        protected sealed override bool OnCanExecute(object parameter)
        {
            if (parameter is T t)
            {
                return this.OnCanExecute(t);
            }
            return false;
        }

        protected sealed override void OnExecute(object parameter)
        {
            if (parameter is T t)
            {
                this.OnExecute(t);
            }
        }

        protected virtual bool OnCanExecute(T obj)
        {
            return false;
        }

        protected virtual void OnExecute(T obj)
        {

        }
    }
}