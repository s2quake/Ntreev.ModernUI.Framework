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
using Ntreev.Library.ObjectModel;
using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace Ntreev.ModernUI.Framework.Dialogs.ViewModels
{
    public class RenameViewModel : ModalDialogBase
    {
        private readonly string currentName;
        private string newName;
        private readonly Func<string, bool> predicate;

        public RenameViewModel(string currentName)
            : this(currentName, item => true)
        {

        }

        public RenameViewModel(string currentName, Func<string, bool> predicate)
        {
            this.Validate(currentName);
            this.currentName = currentName;
            this.newName = currentName;
            this.predicate = predicate;
            this.DisplayName = Resources.Title_Rename;
        }

        public virtual void Rename()
        {
            this.TryClose(this.CanRename);
        }

        public string NewName
        {
            get { return this.newName ?? string.Empty; }
            set
            {
                this.newName = value;
                this.NotifyOfPropertyChange(nameof(this.NewName));
                this.NotifyOfPropertyChange(nameof(this.CanRename));
            }
        }

        public string CurrentName
        {
            get { return this.currentName; }
        }

        public bool CanRename
        {
            get
            {
                if (this.IsProgressing == true || this.NewName == string.Empty)
                    return false;
                if (this.NewName == this.CurrentName)
                    return false;
                if (NameValidator.VerifyName(this.NewName) == false)
                    return false;
                return this.VerifyRename(this.NewName);
            }
        }

        protected virtual bool VerifyRename(string newName)
        {
            if (this.predicate != null)
                return this.predicate(newName);
            return true;
        }

        private void Validate(string currentName)
        {
            if (currentName == null)
                throw new ArgumentNullException(nameof(currentName));
            NameValidator.ValidateName(currentName);
        }
    }
}
