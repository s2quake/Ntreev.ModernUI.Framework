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

using JSSoft.Library.ObjectModel;
using JSSoft.ModernUI.Framework.Properties;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace JSSoft.ModernUI.Framework.Dialogs.ViewModels
{
    public class MoveViewModel : ModalDialogBase
    {
        private readonly string currentPath;
        private string targetPath;
        private readonly Func<string, bool> predicate;

        public MoveViewModel(string currentPath, string[] targetPaths)
            : this(currentPath, targetPaths, item => true)
        {

        }

        public MoveViewModel(string currentPath, string[] targetPaths, Func<string, bool> predicate)
        {
            this.Validate(currentPath, targetPaths);
            this.currentPath = currentPath;
            if (NameValidator.VerifyCategoryPath(currentPath) == true)
                this.CurrentTargetPath = new CategoryName(currentPath).ParentPath;
            else
                this.CurrentTargetPath = new ItemName(currentPath).CategoryPath;
            this.targetPath = CurrentTargetPath;
            this.TargetPaths = targetPaths;
            this.predicate = predicate;
            this.DisplayName = Resources.Title_Move;
        }

        public virtual Task MoveAsync()
        {
            return this.TryCloseAsync(true);
        }

        public override async Task<bool> CanCloseAsync(CancellationToken cancellationToken)
        {
            return await this.Dispatcher.InvokeAsync(() => this.CanMove);
        }

        public string[] TargetPaths { get; }

        public string TargetPath
        {
            get => this.targetPath ?? string.Empty;
            set
            {
                this.targetPath = value;
                this.NotifyOfPropertyChange(nameof(this.TargetPath));
                this.NotifyOfPropertyChange(nameof(this.CanMove));
            }
        }

        public string CurrentTargetPath { get; }

        public string CurrentPath => this.currentPath ?? string.Empty;

        public bool CanMove
        {
            get
            {
                if (this.IsProgressing == true)
                    return false;

                if (NameValidator.VerifyCategoryPath(this.TargetPath) == false)
                    return false;

                if (this.TargetPaths.Contains(this.TargetPath) == false)
                    return false;

                if (this.TargetPath.StartsWith(this.CurrentPath) == true)
                    return false;

                if (this.TargetPath == this.CurrentTargetPath)
                    return false;

                if (this.TargetPath == this.CurrentPath)
                    return false;

                return this.VerifyMove(this.TargetPath);
            }
        }

        protected virtual bool VerifyMove(string targetPath)
        {
            if (this.predicate != null)
                return this.predicate(targetPath);
            return true;
        }

        private void Validate(string currentPath, string[] targetPaths)
        {
            if (currentPath == null)
                throw new ArgumentNullException(nameof(currentPath));
            if (targetPaths == null)
                throw new ArgumentNullException(nameof(targetPaths));
            if (NameValidator.VerifyCategoryPath(currentPath) == false)
                NameValidator.ValidateItemPath(currentPath);

            foreach (var item in targetPaths)
            {
                NameValidator.ValidateCategoryPath(item);
            }
        }
    }
}
