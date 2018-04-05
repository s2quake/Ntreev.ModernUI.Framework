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
using Caliburn.Micro;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shell;
using Ntreev.ModernUI.Framework.Controls;
using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Framework
{
    public class AppWindowManager : WindowManager
    {
        [Import]
        private IAppConfiguration configs = null;
        [Import]
        private ICompositionService compositionService = null;

        public override bool? ShowDialog(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            this.compositionService.SatisfyImportsOnce(rootModel);
            return base.ShowDialog(rootModel, context, settings);
        }

        protected override Page EnsurePage(object model, object view)
        {
            return base.EnsurePage(model, view);
        }

        protected override Window EnsureWindow(object model, object view, bool isDialog)
        {
            var window = view as Window;

            if (window == null)
            {
                window = isDialog == true ? new DialogWindow() : new Window();
                window.Content = new DialogContentControl() { Content = view, };
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
                        this.UpdatePosition(model, window, owner);
                        this.UpdateSize(model, window);
                        this.UpdateState(model, window);
                    };
                }
                else
                {
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                }
            }
            else
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

            window.Closed += Window_Closed;

            return window;
        }

        protected override Window CreateWindow(object rootModel, bool isDialog, object context, IDictionary<string, object> settings)
        {
            var window = base.CreateWindow(rootModel, isDialog, context, settings);

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
            if (this.configs.TryParse<WindowState>(model.GetType(), nameof(WindowState), out WindowState windowState) == true)
            {
                window.WindowState = windowState;
            }
        }

        private void UpdateSize(object model, Window window)
        {
            if (window.ResizeMode.HasFlag(ResizeMode.CanResize) == false)
                return;

            if (this.configs.TryParse<double>(model.GetType(), nameof(window.Width), out double width) == true)
            {
                window.Width = width;
            }
            if (this.configs.TryParse<double>(model.GetType(), nameof(window.Height), out double height) == true)
            {
                window.Height = height;
            }
        }

        private void UpdatePosition(object model, Window window, Window owner)
        {
            var left = double.NaN;
            var top = double.NaN;
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
            if (this.configs.TryParse<double>(model.GetType(), nameof(window.Left), out double l) == true)
            {
                left = l;
            }
            if (this.configs.TryParse<double>(model.GetType(), nameof(window.Top), out double t) == true)
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
                    this.configs[window.DataContext.GetType(), nameof(window.WindowState)] = window.WindowState;
                    this.configs[window.DataContext.GetType(), nameof(window.Width)] = window.Width;
                    this.configs[window.DataContext.GetType(), nameof(window.Height)] = window.Height;
                    this.configs[window.DataContext.GetType(), nameof(window.Left)] = window.Left;
                    this.configs[window.DataContext.GetType(), nameof(window.Top)] = window.Top;
                }
                catch
                {

                }
            }
        }
    }
}
