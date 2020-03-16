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

using Ntreev.ModernUI.Framework.Properties;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class NewCategoryAsyncViewModel : NewCategoryViewModel
    {
        private bool isValid;

        protected NewCategoryAsyncViewModel(string parentPath)
            : base(parentPath)
        {
            this.PropertyChanged += NewCategoryAsyncViewModel_PropertyChanged;
        }

        public async override void Create()
        {
            try
            {
                this.BeginProgress(Resources.Message_CreatingCategory);
                await this.CreateAsync(this.CategoryName);
                this.EndProgress();
                this.TryClose(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        protected sealed override bool VerifyName(string categoryName)
        {
            if (base.VerifyName(categoryName) == false)
                return false;
            return this.isValid;
        }

        protected virtual void VerifyName(string categoryName, Action<bool> isValid)
        {
            isValid(true);
        }

        protected virtual Task CreateAsync(string categoryName)
        {
            return Task.Delay(1);
        }

        private void VerifyAction(bool isValid)
        {
            this.isValid = isValid;
            this.NotifyOfPropertyChange(nameof(this.CanCreate));
        }

        private void NewCategoryAsyncViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.CategoryName))
            {
                this.VerifyName(this.CategoryName, this.VerifyAction);
            }
            else if (e.PropertyName == nameof(this.IsProgressing))
            {
                this.NotifyOfPropertyChange(nameof(this.CanCreate));
            }
        }
    }
}
