using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class ListBoxControlViewModel : ContentBase, IContent
    {
        public ListBoxControlViewModel()
        {
            this.DisplayName = "ListBox";
        }
    }
}
