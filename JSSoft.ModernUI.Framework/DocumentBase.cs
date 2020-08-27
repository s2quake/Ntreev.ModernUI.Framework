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
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework
{
    public abstract class DocumentBase : ScreenBase, IDocument
    {
        private bool isModified;

        protected DocumentBase()
            : this(null)
        {

        }

        protected DocumentBase(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.CloseCommand = new DelegateCommand(CloseCommand_Execute, CloseCommand_CanExecute);
        }

        public sealed override Task TryCloseAsync(bool? dialogResult = default)
        {
            return base.TryCloseAsync(dialogResult);
        }

        public async void Dispose()
        {
            await this.TryCloseAsync(null);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public ICommand CloseCommand { get; }

        public bool IsModified
        {
            get => this.isModified;
            set
            {
                this.Notifier.SetField(ref this.isModified, value, new string[]
                {
                    nameof(this.IsModified),
                    nameof(this.DisplayName)
                });
                this.Notifier.Notify();
            }
        }

        public override string DisplayName
        {
            get
            {
                if (this.IsModified == true)
                    return base.DisplayName + "*";
                return base.DisplayName;
            }
            set => base.DisplayName = value;
        }

        public event EventHandler Disposed;

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override void OnClose()
        {
            base.OnClose();
            this.OnDisposed(EventArgs.Empty);
        }

        protected virtual void OnDisposed(EventArgs e)
        {
            this.Disposed?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            return base.OnDeactivateAsync(close, cancellationToken);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override void OnViewAttached(object view, object context)
        {
            base.OnViewAttached(view, context);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public sealed override object GetView(object context = null)
        {
            return base.GetView(context);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected sealed override void OnViewReady(object view)
        {
            base.OnViewReady(view);
        }

        private async void CloseCommand_Execute(object obj)
        {
            var save = false;
            if (this.IsModified == true)
            {
                var result = await AppMessageBox.ConfirmSaveOnClosingAsync();
                if (result == null)
                    return;

                save = (bool)result;
            }

            this.IsModified = save;
            this.Dispose();
        }

        private bool CloseCommand_CanExecute(object obj)
        {
            if (this.IsProgressing == true)
                return false;
            return true;
        }
    }
}
