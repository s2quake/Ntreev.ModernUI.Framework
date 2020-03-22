using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class ListBoxViewModel : ContentBase, IContent
    {
        public ListBoxViewModel()
        {
            this.DisplayName = "ListBox";
        }
    }
}
