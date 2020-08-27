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

using JSSoft.Library;
using System;
using System.Windows;
using System.Windows.Threading;

namespace JSSoft.ModernUI.Framework
{
    public abstract class ViewAwareBase : Caliburn.Micro.ViewAware, IProgressable, IPropertyNotifier
    {
        private bool isProgressing;
        private string progressMessage;
        private string displayName;

        protected ViewAwareBase()
        {
            this.Notifier = new PropertyNotifier(this);
        }

        protected ViewAwareBase(IServiceProvider serviceProvider)
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

        public virtual string DisplayName
        {
            get => this.displayName ?? string.Empty;
            set
            {
                this.Notifier.SetField(ref this.displayName, value, nameof(DisplayName));
                this.Notifier.Notify();
            }
        }

        public Dispatcher Dispatcher => Application.Current.Dispatcher;

        protected PropertyNotifier Notifier { get; }

        protected IServiceProvider ServiceProvider { get; }
    }
}
