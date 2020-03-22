using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using System;
using System.ComponentModel.Composition;
using Ntreev.ModernUI.Shell.Properties;

namespace Ntreev.ModernUI.Shell.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IShell))]
    [Order(int.MaxValue)]
    class HelpMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public HelpMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = Resources.MenuItem_Help;
        }
    }
}
