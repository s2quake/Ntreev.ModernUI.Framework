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
using System.Windows;
using Caliburn.Micro;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Framework
{
    public abstract class ModalDialogBase : Screen, IModalDialog
    {
        private bool isProgressing;
        private string progressMessage;
        private bool? dialogResult;
        [Import]
        private IServiceProvider serviceProvider = null;

        protected ModalDialogBase()
        {

        }

        protected ModalDialogBase(string displayName)
        {
            this.DisplayName = displayName;
        }

        public bool? ShowDialog()
        {
            return this.Dispatcher.Invoke(() =>
            {
                WindowManager.ShowDialog(this);
                return this.dialogResult;
            });
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
            this.OnProgress();
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
            this.OnProgress();
        }

        public bool IsProgressing
        {
            get { return this.isProgressing; }
        }

        public string ProgressMessage
        {
            get { return this.progressMessage; }
            set
            {
                this.progressMessage = value;
                this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
            }
        }

        public override void TryClose(bool? dialogResult = null)
        {
            this.dialogResult = dialogResult;
            base.TryClose(dialogResult);
        }

        public bool? DialogResult
        {
            get { return this.dialogResult; }
            set { this.dialogResult = value; }
        }

        public Dispatcher Dispatcher
        {
            get { return Application.Current.Dispatcher; }
        }

        public virtual IEnumerable<IToolBarItem> ToolBarItems
        {
            get
            {
                if (this.serviceProvider == null)
                    return Enumerable.Empty<IToolBarItem>();
                return ToolBarItemUtility.GetToolBarItems(this, this.serviceProvider);
            }
        }

        protected virtual void OnProgress()
        {

        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        internal static IWindowManager WindowManager
        {
            get; set;
        }
    }
}
