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

using Caliburn.Micro;
using JSSoft.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace JSSoft.ModernUI.Framework
{
    public abstract class ModalDialogBase : Screen, IModalDialog, IProgressable, IPropertyNotifier
    {
        private string progressMessage;
        private bool isProgressing;
        private bool? dialogResult;
        private MessageBoxResult result;

        protected ModalDialogBase()
        {
            this.Notifier = new PropertyNotifier(this);
        }

        protected ModalDialogBase(IServiceProvider serviceProvider)
        {
            this.Notifier = new PropertyNotifier(this);
            this.ServiceProvider = serviceProvider;
        }

        public async Task<bool?> ShowDialogAsync()
        {
            await AppWindowManager.Current.ShowDialogAsync(this);
            return this.dialogResult;
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

        public MessageBoxResult Result
        {
            get => this.result;
            set
            {
                this.Notifier.SetField(ref this.result, value, nameof(ProgressMessage));
                this.Notifier.Notify();
            }
        }

        public override async Task TryCloseAsync(bool? dialogResult = null)
        {
            this.dialogResult = dialogResult;
            await base.TryCloseAsync(dialogResult);
        }

        public Dispatcher Dispatcher => Application.Current.Dispatcher;

        public virtual IEnumerable<IToolBarItem> ToolBarItems
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
