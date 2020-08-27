using JSSoft.Library;
using JSSoft.ModernUI.Framework;
using JSSoft.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IShell))]
    [Order(1)]
    class ViewMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public ViewMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = Resources.MenuItem_View;
        }
    }
}
