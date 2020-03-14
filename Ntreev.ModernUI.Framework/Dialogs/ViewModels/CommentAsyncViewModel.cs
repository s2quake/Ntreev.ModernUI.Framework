﻿//Released under the MIT License.
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
using System.Threading.Tasks;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class CommentAsyncViewModel : CommentViewModel
    {
        protected CommentAsyncViewModel()
        {
            this.PropertyChanged += CommentAsyncViewModel_PropertyChanged;
        }

        public async override Task ConfirmAsync()
        {
            try
            {
                this.BeginProgress();
                await this.ConfirmAsync(this.Comment);
                this.EndProgress();
                await this.TryCloseAsync(true);
            }
            catch (Exception e)
            {
                this.EndProgress();
                AppMessageBox.ShowError(e);
            }
        }

        protected virtual void Verify(string comment, Action<bool> isValid)
        {
            isValid(true);
        }

        protected virtual Task ConfirmAsync(string comment)
        {
            return Task.Delay(1);
        }

        private void VerifyAction(bool isValid)
        {
            this.NotifyOfPropertyChange(nameof(this.CanConfirm));
        }

        private void CommentAsyncViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.Comment))
            {
                this.Verify(this.Comment, this.VerifyAction);
            }
            else if (e.PropertyName == nameof(this.IsProgressing))
            {
                this.NotifyOfPropertyChange(nameof(this.CanConfirm));
            }
        }
    }
}
