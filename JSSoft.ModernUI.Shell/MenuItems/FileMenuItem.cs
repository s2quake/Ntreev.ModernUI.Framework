using Ntreev.Library;
using Ntreev.ModernUI.Framework;
using Ntreev.ModernUI.Shell.Properties;
using System;
using System.ComponentModel.Composition;

namespace Ntreev.ModernUI.Shell.MenuItems
{
    [Export(typeof(IMenuItem))]
    [ParentType(typeof(IShell))]
    [Order(0)]
    [CategoryDefinition("Settings")]
    [CategoryDefinition("Recent")]
    [CategoryDefinition("Quit")]
    class MessageBoxMenuItem : MenuItemBase
    {
        [ImportingConstructor]
        public MessageBoxMenuItem(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
            this.DisplayName = Resources.MenuItem_File;
        }
    }
}
