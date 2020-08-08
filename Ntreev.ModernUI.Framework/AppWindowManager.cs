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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework
{
    public sealed class AppWindowManager : WindowManager
    {
        internal AppWindowManager()
        {

        }

        public override Task<bool?> ShowDialogAsync(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            return base.ShowDialogAsync(rootModel, context, settings);
        }

        public static AppWindowManager Current { get; } = new AppWindowManager();

        protected override Page EnsurePage(object model, object view)
        {
            return base.EnsurePage(model, view);
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            if (view is Window window)
            {
                var owner = InferOwnerOf(window);
                if (owner != null && isDialog)
                {
                    window.Owner = owner;

                    window.Loaded += (s, e) =>
                    {
                        if (owner.WindowState != WindowState.Maximized)
                        {
                            window.Left = owner.Left + (owner.ActualWidth - window.ActualWidth) / 2;
                            window.Top = owner.Top + (owner.ActualHeight - window.ActualHeight) / 2;
                        }
                        else
                        {
                            window.Left = (owner.ActualWidth - window.ActualWidth) / 2;
                            window.Top = (owner.ActualHeight - window.ActualHeight) / 2;
                        }
                        window.SizeToContent = SizeToContent.Manual;
                    };
                }
            }
            else
            {
                window = isDialog == true ? new DialogWindow() : new Window();
                window.Content = view;
                window.SetValue(View.IsGeneratedProperty, true);

                var owner = this.InferOwnerOf(window);
#if DEBUG
                if (owner == null)
                    owner = Application.Current.Windows[0];
#endif
                if (owner != null)
                {
                    window.SizeToContent = SizeToContent.Manual;
                    window.WindowStartupLocation = WindowStartupLocation.Manual;
                    window.Owner = owner;

                    window.Loaded += (s, e) =>
                    {
                        if (Keyboard.IsKeyDown(Key.LeftShift) == false)
                        {
                            this.UpdatePosition(model, window, owner);
                            this.UpdateSize(model, window);
                            this.UpdateState(model, window);
                        }
                        if (window is DialogWindow dialogWindow)
                        {
                            dialogWindow.Dispatcher.InvokeAsync(() => dialogWindow.IsEnsured = true);
                        }
                    };
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }

            window.Closed += Window_Closed;

            return window;
        }

        protected override async Task<Window> CreateWindowAsync(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            var window = await base.CreateWindowAsync(rootModel, isDialog, context, settings);

            if (window.Owner != null)
            {
                window.ShowInTaskbar = false;
            }

            if (isDialog == true)
            {
                if (rootModel is IModalDialog == true)
                {
                    if (string.IsNullOrEmpty(((IModalDialog)rootModel).DisplayName) == true)
                        window.Title = string.Empty;
                    else
                        window.Title = ((IModalDialog)rootModel).DisplayName;
                }
            }

            return window;
        }

        /// <summary>
        /// Loaded 이벤트 이후에 한번 ChromeWorker라는 부분에서 크기 변경이 일어나기 때문에 윈도우 위치를 다시 계산함
        /// </summary>
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var window = sender as Window;
            var owner = window.Owner;
            window.SizeChanged -= Window_SizeChanged;
            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Left = owner.Left + (owner.Width - window.ActualWidth) / 2;
            window.Top = owner.Top + (owner.Height - window.ActualHeight) / 2;
            window.SizeToContent = SizeToContent.Manual;
        }

        private void UpdateState(object model, Window window)
        {
            if (AppConfiguration.Current.TryGetValue<WindowState>(typeof(AppWindowManager), model.GetType(), nameof(WindowState), out var windowState) == true)
            {
                window.WindowState = windowState;
            }
        }

        private void UpdateSize(object model, Window window)
        {
            if (window.ResizeMode.HasFlag(ResizeMode.CanResize) == false)
                return;

            if (AppConfiguration.Current.TryGetValue<double>(typeof(AppWindowManager), model.GetType(), nameof(window.Width), out var width) == true)
            {
                window.Width = width;
            }
            if (AppConfiguration.Current.TryGetValue<double>(typeof(AppWindowManager), model.GetType(), nameof(window.Height), out var height) == true)
            {
                window.Height = height;
            }
        }

        private void UpdatePosition(object model, Window window, Window owner)
        {
            double left;
            double top;
            if (owner.WindowState != WindowState.Maximized)
            {
                left = owner.Left + (owner.ActualWidth - window.ActualWidth) / 2;
                top = window.Top = owner.Top + (owner.ActualHeight - window.ActualHeight) / 2;
            }
            else
            {
                left = (owner.ActualWidth - window.ActualWidth) / 2;
                top = (owner.ActualHeight - window.ActualHeight) / 2;
            }
            if (AppConfiguration.Current.TryGetValue<double>(typeof(AppWindowManager), model.GetType(), nameof(window.Left), out var l) == true)
            {
                left = l;
            }
            if (AppConfiguration.Current.TryGetValue<double>(typeof(AppWindowManager), model.GetType(), nameof(window.Top), out var t) == true)
            {
                top = t;
            }

            window.SizeToContent = SizeToContent.WidthAndHeight;
            window.Left = left;
            window.Top = top;
            window.SizeToContent = SizeToContent.Manual;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (sender is DialogWindow window)
            {
                try
                {
                    AppConfiguration.Current.SetValue(typeof(AppWindowManager), window.DataContext.GetType(), nameof(window.WindowState), window.WindowState);
                    AppConfiguration.Current.SetValue(typeof(AppWindowManager), window.DataContext.GetType(), nameof(window.Width), window.Width);
                    AppConfiguration.Current.SetValue(typeof(AppWindowManager), window.DataContext.GetType(), nameof(window.Height), window.Height);
                    AppConfiguration.Current.SetValue(typeof(AppWindowManager), window.DataContext.GetType(), nameof(window.Left), window.Left);
                    AppConfiguration.Current.SetValue(typeof(AppWindowManager), window.DataContext.GetType(), nameof(window.Top), window.Top);
                }
                catch
                {

                }
            }
        }
    }
}
