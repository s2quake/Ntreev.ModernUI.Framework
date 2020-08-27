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

using JSSoft.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace JSSoft.ModernUI.Framework
{
    public abstract class ViewModelBase : Caliburn.Micro.PropertyChangedBase, IProgressable, IPropertyNotifier
    {
        private bool isProgressing;
        private string progressMessage;

        protected ViewModelBase()
        {
            this.Notifier = new PropertyNotifier(this);
        }

        protected ViewModelBase(IServiceProvider serviceProvider)
        {
            this.Notifier = new PropertyNotifier(this);
            this.ServiceProvider = serviceProvider;
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

        public virtual IEnumerable<IMenuItem> ContextMenus
        {
            get
            {
                if (this.ServiceProvider != null)
                    return MenuItemUtility.GetMenuItems(this, this.ServiceProvider);
                return Enumerable.Empty<IMenuItem>();
            }
        }

        public virtual IEnumerable<IToolBarItem> ToolBarMenus
        {
            get
            {
                if (this.ServiceProvider != null)
                    return ToolBarItemUtility.GetToolBarItems(this, this.ServiceProvider);
                return Enumerable.Empty<IToolBarItem>();
            }
        }

        protected PropertyNotifier Notifier { get; }

        protected IServiceProvider ServiceProvider { get; }
    }
}
