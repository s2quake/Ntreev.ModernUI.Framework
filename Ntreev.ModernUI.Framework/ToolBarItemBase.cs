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
using Ntreev.ModernUI.Framework.Controls;
using System;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Ntreev.ModernUI.Framework
{
    public abstract class ToolBarItemBase : PropertyChangedBase, IToolBarItem, ICommand
    {
        private string displayName;
        private EventHandler canExecuteChanged;
        private object icon;

        protected ToolBarItemBase()
        {
            CommandManager.RequerySuggested += this.InvokeCanExecuteChangedEvent;
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

        public bool IsVisible
        {
            get
            {
                if (this.HideOnDisabled == true && this.IsEnabled == false)
                    return false;
                return true;
            }
        }

        public bool IsEnabled { get; private set; }

        public object Icon
        {
            get
            {
                if (this.icon is string uri)
                {
                    if (uri.StartsWith("pack://application:,,,") == false)
                    {
                        if (uri.StartsWith("/") == true)
                            uri = "pack://application:,,," + uri;
                        else
                            uri = "pack://application:,,,/" + uri;
                    }
                    return new IconButton()
                    {
                        Source = new BitmapImage(new Uri(uri)),
                    };
                }
                else if (this.icon is Type viewType)
                {
                    return ViewLocator.GetOrCreateViewType(viewType);
                }
                else
                {
                    return this.icon;
                }
            }
            set
            {
                this.icon = value;
                this.NotifyOfPropertyChange(nameof(this.Icon));
            }
        }

        public bool HideOnDisabled { get; set; }

        protected virtual void OnExecute(object parameter)
        {

        }

        protected virtual bool OnCanExecute(object parameter) => true;

        protected void InvokeCanExecuteChangedEvent() => this.canExecuteChanged?.Invoke(this, EventArgs.Empty);

        protected void InvokeCanExecuteChangedEvent(object sender, EventArgs e) => this.canExecuteChanged?.Invoke(this, EventArgs.Empty);

        #region ICommand

        event EventHandler ICommand.CanExecuteChanged
        {
            add => this.canExecuteChanged += value;
            remove => this.canExecuteChanged -= value;
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

        void ICommand.Execute(object parameter) => this.OnExecute(parameter);

        #endregion
    }
}