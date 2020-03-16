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
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;

namespace Ntreev.ModernUI.Framework
{
    public abstract class ViewModelBase : Caliburn.Micro.PropertyChangedBase
    {
        private bool isProgressing;
        private string progressMessage;
        private readonly IServiceProvider serviceProvider;

        protected ViewModelBase()
        {

        }

        protected ViewModelBase(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
            if (this.serviceProvider.GetService(typeof(ICompositionService)) is ICompositionService compositionService)
            {
                this.Dispatcher.InvokeAsync(this.OnImportsSatisfied);
            }
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

        public virtual IEnumerable<IMenuItem> ContextMenus => MenuItemUtility.GetMenuItems(this, this.serviceProvider ?? AppBootstrapperBase.Current);

        public virtual IEnumerable<IToolBarItem> ToolBarMenus => ToolBarItemUtility.GetToolBarItems(this, this.serviceProvider ?? AppBootstrapperBase.Current);

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
    }
}
