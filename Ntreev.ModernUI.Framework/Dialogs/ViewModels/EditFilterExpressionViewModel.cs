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

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class EditFilterExpressionViewModel : ModalDialogBase
    {
        private string filterExpression;
        private string filterExpressions;

        public EditFilterExpressionViewModel()
        {
            this.DisplayName = Resources.Title_EditFilter;
        }

        public void Close()
        {
            this.TryClose(true);
        }

        public string FilterExpression
        {
            get => this.filterExpression ?? string.Empty;
            set
            {
                this.filterExpression = value ?? string.Empty;
                this.filterExpressions = this.filterExpression.Replace(";", Environment.NewLine);

                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
                this.NotifyOfPropertyChange(nameof(this.FilterExpressions));
            }
        }

        public string FilterExpressions
        {
            get => this.filterExpressions ?? string.Empty;
            set
            {
                this.filterExpressions = value ?? string.Empty;
                this.filterExpression = this.filterExpressions.Trim().Replace(Environment.NewLine, ";");
                this.NotifyOfPropertyChange(nameof(this.FilterExpression));
                this.NotifyOfPropertyChange(nameof(this.FilterExpressions));
            }
        }
    }
}
