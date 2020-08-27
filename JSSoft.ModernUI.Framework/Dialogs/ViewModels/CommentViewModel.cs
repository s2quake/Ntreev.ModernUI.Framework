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

using System.ComponentModel;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Framework.Dialogs.ViewModels
{
    public class CommentViewModel : ModalDialogBase
    {
        private string comment;
        private string commentHeader;

        public CommentViewModel()
        {
            this.PropertyChanged += CommentViewModel_PropertyChanged;
        }

        public virtual Task ConfirmAsync()
        {
            return this.TryCloseAsync(true);
        }

        public string Comment
        {
            get => this.comment ?? string.Empty;
            set
            {
                this.comment = value;
                this.NotifyOfPropertyChange(nameof(this.Comment));
                this.NotifyOfPropertyChange(nameof(this.CanConfirm));
            }
        }

        public bool CanConfirm
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;

                if (this.Comment == string.Empty || this.AllowEmptyComment == true)
                    return false;

                return true;
            }
        }

        public bool AllowEmptyComment { get; set; }

        public string CommentHeader
        {
            get => this.commentHeader ?? string.Empty;
            set
            {
                this.commentHeader = value;
                this.NotifyOfPropertyChange(nameof(this.CommentHeader));
            }
        }

        private void CommentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.IsProgressing))
            {
                //this.NotifyOfPropertyChange(nameof(this.CanClose));
            }
        }
    }
}
