﻿// Released under the MIT License.
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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace JSSoft.ModernUI.Framework
{
    public abstract class ScreenBase : Caliburn.Micro.Screen, IProgressable, IPropertyNotifier
    {
        private bool isProgressing;
        private string progressMessage;

        protected ScreenBase()
        {
            this.Notifier = new PropertyNotifier(this);
        }

        protected ScreenBase(IServiceProvider serviceProvider)
        {
            this.Notifier = new PropertyNotifier(this);
            this.ServiceProvider = serviceProvider;
        }

        public sealed override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
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
            this.Notifier.SetField(ref this.isProgressing, true, nameof(IsProgressing));
            this.Notifier.SetField(ref this.progressMessage, message, nameof(ProgressMessage));
            this.Notifier.Notify();
        }

        public void EndProgress()
        {
            this.EndProgress(string.Empty);
        }

        public void EndProgress(string message)
        {
            this.Notifier.SetField(ref this.isProgressing, false, nameof(IsProgressing));
            this.Notifier.SetField(ref this.progressMessage, message, nameof(ProgressMessage));
            this.Notifier.Notify();
        }

        public bool IsProgressing
        {
            get => this.isProgressing;
            set
            {
                this.Notifier.SetField(ref this.isProgressing, value, nameof(IsProgressing));
                this.Notifier.Notify();
            }
        }

        public string ProgressMessage
        {
            get => this.progressMessage ?? string.Empty;
            set
            {
                this.Notifier.SetField(ref this.progressMessage, value, nameof(ProgressMessage));
                this.Notifier.Notify();
            }
        }

        public Dispatcher Dispatcher => Application.Current.Dispatcher;

        public IEnumerable<IMenuItem> MenuItems => MenuItemUtility.GetMenuItems(this, this.ServiceProvider);

        public IEnumerable<IToolBarItem> ToolBarItems => ToolBarItemUtility.GetToolBarItems(this, this.ServiceProvider);

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            if (view is UIElement element)
            {
                if (AppBootstrapperBase.Current.GetService(typeof(IEnumerable<IMenuItem>)) is IEnumerable<IMenuItem> menuItems)
                {
                    var items = MenuItemUtility.GetMenuItems(this, menuItems);
                    foreach (var item in items)
                    {
                        this.SetInputBindings(element, item);
                    }
                }
                if (AppBootstrapperBase.Current.GetService(typeof(IEnumerable<IToolBarItem>)) is IEnumerable<IToolBarItem> toolbarItems)
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

        protected void BuildUp(object instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (this.ServiceProvider == null)
                throw new InvalidOperationException();
            if (this.ServiceProvider.GetService(typeof(IBuildUp)) is IBuildUp buildUp)
            {
                buildUp.BuildUp(this);
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        protected PropertyNotifier Notifier { get; }

        protected IServiceProvider ServiceProvider { get; }

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

            foreach (var item in menuItem.MenuItems)
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
