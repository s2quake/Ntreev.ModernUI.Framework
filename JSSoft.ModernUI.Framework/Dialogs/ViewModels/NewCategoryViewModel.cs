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

using JSSoft.Library.IO;
using JSSoft.Library.ObjectModel;
using JSSoft.ModernUI.Framework.Properties;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Framework.Dialogs.ViewModels
{
    public class NewCategoryViewModel : ModalDialogBase
    {
        private readonly Func<string, bool> predicate;
        private string categoryName;

        public NewCategoryViewModel(string parentPath)
            : this(parentPath, item => true)
        {

        }

        public NewCategoryViewModel(string parentPath, string[] categoryNames)
            : this(parentPath, item => categoryNames.Contains(item) == false)
        {

        }

        public NewCategoryViewModel(string parentPath, Func<string, bool> predicate)
        {
            this.Validate(parentPath);
            this.DisplayName = Resources.Title_NewCategory;
            this.ParentPath = parentPath;
            this.predicate = predicate;
        }

        public virtual Task CreateAsync()
        {
            return this.TryCloseAsync(true);
        }

        public string CategoryName
        {
            get => this.categoryName ?? string.Empty;
            set
            {
                this.categoryName = value;
                this.NotifyOfPropertyChange(nameof(this.CategoryName));
                this.NotifyOfPropertyChange(nameof(this.CategoryPath));
                this.NotifyOfPropertyChange(nameof(this.CanCreate));
            }
        }

        public string CategoryPath
        {
            get
            {
                if (this.CategoryName == string.Empty)
                    return this.ParentPath + this.CategoryName;
                return this.ParentPath + this.CategoryName + PathUtility.Separator;
            }
        }

        public string ParentPath { get; }

        public bool CanCreate
        {
            get
            {
                if (this.IsProgressing == true || this.CategoryName == string.Empty)
                    return false;
                if (NameValidator.VerifyName(this.CategoryName) == false)
                    return false;
                return this.VerifyName(this.CategoryName);
            }
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return await this.Dispatcher.InvokeAsync(() => this.IsProgressing == false);
        }

        protected virtual bool VerifyName(string categoryName)
        {
            if (this.predicate != null)
                return this.predicate(categoryName);
            return true;
        }

        private void Validate(string parentPath)
        {
            if (parentPath == null)
                throw new ArgumentNullException(nameof(parentPath));
            NameValidator.ValidateCategoryPath(parentPath);
        }
    }
}
