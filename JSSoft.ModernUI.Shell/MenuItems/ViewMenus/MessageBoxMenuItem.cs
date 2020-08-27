using JSSoft.ModernUI.Framework;
using System;
using System.ComponentModel.Composition;

namespace JSSoft.ModernUI.Shell.MenuItems.ViewMenus
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(ViewMenuItem))]
    class MessageBoxMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public MessageBoxMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = "MessageBox";
        }
    }
}
