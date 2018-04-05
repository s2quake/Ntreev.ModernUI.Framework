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
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ntreev.ModernUI.Framework.Properties;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class RenameAsyncViewModel : RenameViewModel
    {
        private bool isValid;

        public RenameAsyncViewModel(string currentName)
            : base(currentName)
        {
            this.PropertyChanged += RenameAsyncViewModel_PropertyChanged;
        }

        public async sealed override void Rename()
        {
            try
            {
                this.BeginProgress(Resources.Message_Renaming);
                await this.RenameAsync(this.NewName);
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        protected sealed override bool VerifyRename(string newName)
        {
            if (base.VerifyRename(newName) == false)
                return false;
            return this.isValid;
        }

        protected virtual void VerifyRename(string newName, Action<bool> isValid)
        {
            isValid(true);
        }

        protected virtual Task RenameAsync(string newName)
        {
            return Task.Delay(1);
        }

        protected override void OnProgress()
        {
            base.OnProgress();
            this.NotifyOfPropertyChange(nameof(this.CanRename));
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanRename));
        }

        private void RenameAsyncViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.NewName))
            {
                this.VerifyRename(this.NewName, this.VerifyAction);
            }
        }
    }
}
