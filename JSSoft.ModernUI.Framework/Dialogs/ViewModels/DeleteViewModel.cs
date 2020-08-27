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

using JSSoft.ModernUI.Framework.Properties;
using System.Threading;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Framework.Dialogs.ViewModels
{
    public class DeleteViewModel : ModalDialogBase
    {
        private readonly string textToDelete;
        private string deletionMessage;
        private string comment;
        private string target;

        public DeleteViewModel()
        {
            this.DisplayName = Resources.Title_Delete;
            this.textToDelete = Resources.Text_DeleteNow;
            this.comment = string.Format(Resources.Comment_TypeText, this.textToDelete);
        }

        public string DeletionMessage
        {
            get => this.deletionMessage ?? string.Empty;
            set
            {
                this.deletionMessage = value;
                this.NotifyOfPropertyChange(nameof(this.DeletionMessage));
                this.NotifyOfPropertyChange(nameof(this.CanDelete));
            }
        }

        public string Comment
        {
            get => this.comment ?? string.Empty;
            set
            {
                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.Comment));
            }
        }

        public virtual Task DeleteAsync()
        {
            return this.TryCloseAsync(true);
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return await this.Dispatcher.InvokeAsync(() => this.CanDelete);
        }

        public bool CanDelete => this.deletionMessage == this.textToDelete;

        public string Target
        {
            get => this.target ?? string.Empty;
            set
            {
                this.target = value;
                this.NotifyOfPropertyChange(nameof(this.Target));
            }
        }
    }
}
