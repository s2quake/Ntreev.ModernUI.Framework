using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.Contents.ViewModels
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
