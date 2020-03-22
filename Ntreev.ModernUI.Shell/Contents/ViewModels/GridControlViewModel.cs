using Ntreev.ModernUI.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class GridControlViewModel : ContentBase, IContent
    {
        public GridControlViewModel()
        {
            this.DisplayName = "GridControl";
        }
    }
}
