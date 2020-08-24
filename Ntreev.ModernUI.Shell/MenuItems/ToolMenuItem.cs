using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Shell.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IShell))]
    [Order(2)]
    class ToolMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public ToolMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = Resources.MenuItem_Tool;
        }
    }
}
