using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Shell.Contents.ViewModels
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
