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

using Ntreev.ModernUI.Framework.Dialogs.Views;
using Ntreev.ModernUI.Framework.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    [View(typeof(MoveView))]
    public class MoveAsyncViewModel : MoveViewModel
    {
        private bool isValid;

        public MoveAsyncViewModel(string currentPath, string[] targetPaths)
            : base(currentPath, targetPaths)
        {
            this.PropertyChanged += MoveAsyncViewModel_PropertyChanged;
        }

        public async sealed override void Move()
        {
            try
            {
                this.BeginProgress(Resources.Message_Moving);
                await this.MoveAsync(this.TargetPath);
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        protected sealed override bool VerifyMove(string targetPath)
        {
            if (base.VerifyMove(targetPath) == false)
                return false;
            return this.isValid;
        }

        protected virtual void VerifyMove(string targetPath, Action<bool> isValid)
        {
            isValid(true);
        }

        protected virtual Task MoveAsync(string targetPath)
        {
            return Task.Delay(1);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanMove));
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanMove));
        }

        private void MoveAsyncViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.TargetPath))
            {
                this.VerifyMove(this.TargetPath, this.VerifyAction);
            }
        }
    }
}
