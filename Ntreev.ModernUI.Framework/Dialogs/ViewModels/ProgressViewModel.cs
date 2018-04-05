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

using Ntreev.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class ProgressViewModel : ModalDialogBase
    {
        private Action action;

        public ProgressViewModel()
        {

        }

        public void ShowDialog(Action action)
        {
            this.action = () => this.Initialize(action);
            base.ShowDialog();
        }

        public void ShowDialog(Task task)
        {
            this.action = () => this.Initialize(task);
            base.ShowDialog();
        }

        public void OK()
        {
            this.TryClose(null);
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            this.action?.Invoke();
        }

        private async void Initialize(Action action)
        {
            this.BeginProgress();
            await Task.Delay(100);
            try
            {
                action();
                this.EndProgress();
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        private async void Initialize(Task task)
        {
            this.BeginProgress();
            await Task.Delay(100);
            try
            {
                await task;
                this.EndProgress();
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }
    }
}
