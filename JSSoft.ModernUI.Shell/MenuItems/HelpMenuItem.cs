using JSSoft.Library;
using JSSoft.ModernUI.Framework;
using JSSoft.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.MenuItems
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
