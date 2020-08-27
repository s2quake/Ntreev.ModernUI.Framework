using System.Collections.Generic;

namespace JSSoft.ModernUI.Shell
{
    public interface IShell
    {
        IEnumerable<IContent> Contents { get; }

        IContent SelectedContent { get; set; }
    }
}
