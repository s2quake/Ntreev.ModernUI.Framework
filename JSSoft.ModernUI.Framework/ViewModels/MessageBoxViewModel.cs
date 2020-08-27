// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using Caliburn.Micro;
using System.Threading.Tasks;
using System.Windows;

namespace JSSoft.ModernUI.Framework.ViewModels
{
    class MessageBoxViewModel : Screen, IModalDialog
    {
        private string progressMessage;
        private bool isProgressing;
        private string message;
        private MessageBoxResult result;

        public MessageBoxViewModel()
        {

        }

        public Task OKAsync()
        {
            return this.SelectAsync(MessageBoxResult.OK);
        }

        public Task CancelAsync()
        {
            return this.SelectAsync(MessageBoxResult.Cancel);
        }

        public Task YesAsync()
        {
            return this.SelectAsync(MessageBoxResult.Yes);
        }

        public Task NoAsync()
        {
            return this.SelectAsync(MessageBoxResult.No);
        }

        public string Message
        {
            get => this.message;
            set
            {
                this.message = value;
                this.NotifyOfPropertyChange(nameof(Message));
            }
        }

        public MessageBoxResult Result
        {
            get => this.result;
            private set
            {
                this.result = value;
                this.NotifyOfPropertyChange(nameof(Result));
            }
        }

        public MessageBoxButton Button { get; set; }

        public MessageBoxImage Image { get; set; }

        public bool IsProgressing
        {
            get => this.isProgressing;
            set
            {
                this.isProgressing = value;
                this.NotifyOfPropertyChange(nameof(this.IsProgressing));
            }
        }

        public string ProgressMessage
        {
            get => this.progressMessage ?? string.Empty;
            set
            {
                this.progressMessage = value;
                this.NotifyOfPropertyChange(nameof(this.ProgressMessage));
            }
        }

        private async Task SelectAsync(MessageBoxResult result)
        {
            bool? dialogResult = null;
            this.Result = result;

            switch (this.Button)
            {
                case MessageBoxButton.OK:
                    dialogResult = true;
                    break;
                case MessageBoxButton.OKCancel:
                    dialogResult = result == MessageBoxResult.OK;
                    break;
                case MessageBoxButton.YesNo:
                    dialogResult = result == MessageBoxResult.Yes;
                    break;
                case MessageBoxButton.YesNoCancel:
                    if (result == MessageBoxResult.Yes)
                        dialogResult = true;
                    else if (result == MessageBoxResult.No)
                        dialogResult = false;
                    break;
            }
            await this.TryCloseAsync(dialogResult);
        }
    }
}
