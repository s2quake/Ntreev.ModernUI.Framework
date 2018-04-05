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

using System.Linq;
using Ntreev.ModernUI.Framework.Properties;
using System;
using System.Threading.Tasks;
using Ntreev.Library.ObjectModel;
using System.Windows.Threading;
using Ntreev.ModernUI.Framework.Dialogs.ViewModels;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class RenameCategoryViewModel : RenameViewModel
    {
        private readonly string parentPath;
        private readonly string[] categoryPaths;

        public RenameCategoryViewModel(string categoryPath, string[] categoryPaths)
            : base(new CategoryName(categoryPath).Name)
        {
            this.Validate(categoryPath, categoryPaths);
            this.parentPath = new CategoryName(categoryPath).ParentPath;
            this.categoryPaths = categoryPaths;
        }

        protected override bool VerifyRename(string newName)
        {
            if (base.VerifyRename(newName) == false)
                return false;
            if (this.categoryPaths.Contains(this.parentPath + this.NewName) == true)
                return false;
            return true;
        }

        private void Validate(string categoryPath, string[] categoryPaths)
        {
            if (categoryPath == null)
                throw new ArgumentNullException(nameof(categoryPath));
            if (categoryPath == null)
                throw new ArgumentNullException(nameof(categoryPaths));
            NameValidator.ValidateCategoryPath(categoryPath);
            foreach (var item in categoryPaths)
            {
                NameValidator.ValidateCategoryPath(item);
            }
        }
    }
}