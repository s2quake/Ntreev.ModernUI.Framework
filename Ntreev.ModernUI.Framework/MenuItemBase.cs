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
using Ntreev.Library.Linq;
using Caliburn.Micro;
using System.Windows.Input;
using System.ComponentModel.Composition;
using System.Windows;
using System.Collections;
using System.Globalization;
using System.ComponentModel.Composition.Hosting;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Ntreev.ModernUI.Framework.Controls;

namespace Ntreev.ModernUI.Framework
{
    public abstract class MenuItemBase : PropertyChangedBase, IMenuItem, ICommand
    {
        private bool isChecked;
        private string displayName;
        private EventHandler canExecuteChanged;
        private object icon;

        protected MenuItemBase(IServiceProvider serviceProvider)
        {
            this.ServiceProvider = serviceProvider;
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

        public virtual IEnumerable<IMenuItem> ItemsSource
        {
            get
            {
                if (this.ServiceProvider != null)
                {
                    foreach (var item in MenuItemUtility.GetMenuItems<IMenuItem>(this, this.ServiceProvider))
                    {
                        yield return item;
                    }
                }
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

        protected IServiceProvider ServiceProvider { get; }

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
}