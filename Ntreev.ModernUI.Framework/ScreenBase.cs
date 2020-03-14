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
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
    public abstract class ScreenBase : Caliburn.Micro.Screen
    {
        private bool isProgressing;
        private string progressMessage;
        private readonly IServiceProvider serviceProvider;

        protected ScreenBase()
        {

        }

        protected ScreenBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            if (this.serviceProvider.GetService(typeof(ICompositionService)) is ICompositionService compositionService)
            {
                this.Dispatcher.InvokeAsync(this.OnImportsSatisfied);
            }
        }

        public sealed async override Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            if (await this.CloseAsync() == false)
                return false;
            foreach (var item in this.Views)
            {
                if (item.Value is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            this.OnClose();
            return true;
        }

        public void BeginProgress()
        {
            this.BeginProgress(string.Empty);
        }

        public void BeginProgress(string message)
        {
            this.isProgressing = true;
            this.progressMessage = message;
            this.NotifyOfPropertyChange(nameof(this.IsProgressing));
            this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
        }

        public void EndProgress()
        {
            this.EndProgress(string.Empty);
        }

        public void EndProgress(string message)
        {
            this.isProgressing = false;
            this.progressMessage = message;
            this.NotifyOfPropertyChange(nameof(this.IsProgressing));
            this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
        }

        public bool IsProgressing
        {
            get => this.isProgressing;
            set
            {
                this.isProgressing = value;
                this.NotifyOfPropertyChange(nameof(this.IsProgressing));
            }
        }

        public string ProgressMessage
        {
            get => this.progressMessage ?? string.Empty;
            set
            {
                this.progressMessage = value;
                this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
            }
        }

        public Dispatcher Dispatcher => Application.Current.Dispatcher;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is UIElement element)
            {
                if (this.serviceProvider.GetService(typeof(IEnumerable<IMenuItem>)) is IEnumerable<IMenuItem> menuItems)
                {
                    var items = MenuItemUtility.GetMenuItems(this, menuItems);
                    foreach (var item in items)
                    {
                        this.SetInputBindings(element, item);
                    }
                }
                if (this.serviceProvider.GetService(typeof(IEnumerable<IToolBarItem>)) is IEnumerable<IToolBarItem> toolbarItems)
                {
                    var items = ToolBarItemUtility.GetToolBarItems(this, toolbarItems);
                    foreach (var item in items)
                    {
                        this.SetInputBindings(element, item);
                    }
                }
            }
        }

        protected override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        protected override void OnViewReady(object view)
        {
            base.OnViewReady(view);
        }

        protected virtual void OnClose()
        {

        }

        protected virtual async Task<bool> CloseAsync()
        {
            return await Task.Run(() => this.IsProgressing == false);
        }

        protected void SatisfyImportsOnce(object attributedPart)
        {
            if (this.serviceProvider.GetService(typeof(ICompositionService)) is ICompositionService compositionService)
            {
                compositionService.SatisfyImportsOnce(attributedPart);
            }
        }

        protected virtual void OnImportsSatisfied()
        {

        }

        private void SetInputBindings(UIElement element, IMenuItem menuItem)
        {
            if (menuItem.InputGesture != null)
            {
                if (menuItem.IsVisible == true)
                    element.InputBindings.Add(new InputBinding(menuItem.Command, menuItem.InputGesture));

                if (menuItem is INotifyPropertyChanged notifyObject)
                {
                    notifyObject.PropertyChanged += (s, e) =>
                    {
                        if (menuItem.IsVisible == true)
                        {
                            for (var i = 0; i < element.InputBindings.Count; i++)
                            {
                                if (element.InputBindings[i].Command == menuItem)
                                {
                                    return;
                                }
                            }
                            element.InputBindings.Add(new InputBinding(menuItem.Command, menuItem.InputGesture));
                        }
                        else
                        {
                            for (var i = 0; i < element.InputBindings.Count; i++)
                            {
                                if (element.InputBindings[i].Command == menuItem)
                                {
                                    element.InputBindings.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    };
                }
            }

            foreach (var item in menuItem.ItemsSource)
            {
                this.SetInputBindings(element, item);
            }
        }

        private void SetInputBindings(UIElement element, IToolBarItem toolbarItem)
        {
            if (toolbarItem.InputGesture != null)
            {
                if (toolbarItem.IsVisible == true)
                    element.InputBindings.Add(new InputBinding(toolbarItem.Command, toolbarItem.InputGesture));

                if (toolbarItem is INotifyPropertyChanged notifyObject)
                {
                    notifyObject.PropertyChanged += (s, e) =>
                    {
                        if (toolbarItem.IsVisible == true)
                        {
                            for (var i = 0; i < element.InputBindings.Count; i++)
                            {
                                if (element.InputBindings[i].Command == toolbarItem)
                                {
                                    return;
                                }
                            }
                            element.InputBindings.Add(new InputBinding(toolbarItem.Command, toolbarItem.InputGesture));
                        }
                        else
                        {
                            for (var i = 0; i < element.InputBindings.Count; i++)
                            {
                                if (element.InputBindings[i].Command == toolbarItem)
                                {
                                    element.InputBindings.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                    };
                }
            }
        }
    }
}
