//Released under the MIT License.
//
//Copyright (c) 2018 Ntreev Soft co., Ltd.
//
//Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
//documentation files (the "Software"), to deal in the Software without restriction, including without limitation the 
//rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
//persons to whom the Software is furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in all copies or substantial portions of the 
//Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE 
//WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR 
//COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR 
//OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    class ModernContextMenuItem : MenuItem
    {
        private readonly ModernContextMenu contextMenu;

        static ModernContextMenuItem()
        {
            CommandProperty.OverrideMetadata(typeof(ModernContextMenuItem),
                new FrameworkPropertyMetadata(null, CommandPropertyChangedCallback, CommandPropertyCoerceValueCallback));
            CommandParameterProperty.OverrideMetadata(typeof(ModernContextMenuItem),
                new FrameworkPropertyMetadata(null, CommandParameterPropertyChangedCallback));
        }

        public ModernContextMenuItem(ModernContextMenu contextMenu)
        {
            this.contextMenu = contextMenu;
            this.contextMenu.Opened += ContextMenu_Opened;
            this.contextMenu.Closed += ContextMenu_Closed;

            BindingOperations.SetBinding(this, CommandParameterProperty, new Binding(nameof(this.DataContext)) { Source = contextMenu, });
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ModernContextMenuItem(this.contextMenu);
        }

        private static object CommandPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            var self = d as ModernContextMenuItem;
            if (self.contextMenu.IsOpen == false)
                return null;
            var parameter = d.GetValue(CommandParameterProperty);
            if (parameter == null)
                return null;
            return baseValue;
        }

        private static void CommandPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void CommandParameterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(CommandProperty);
        }

        private void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            this.CoerceValue(CommandProperty);
        }

        private void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            this.CoerceValue(CommandProperty);
        }
    }
}
