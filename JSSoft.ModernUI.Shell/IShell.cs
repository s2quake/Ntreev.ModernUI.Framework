using System.Collections.Generic;

namespace Ntreev.ModernUI.Shell
{
    public interface IShell
    {
        IEnumerable<IContent> Contents { get; }

        IContent SelectedContent { get; set; }
    }
}
