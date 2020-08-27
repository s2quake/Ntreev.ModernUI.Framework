using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.Contents.ViewModels
{
    [Export(typeof(IContent))]
    class TerminalViewModel : ContentBase, IContent
    {
        public TerminalViewModel()
        {
            this.DisplayName = "Terminal";
        }
    }
}
