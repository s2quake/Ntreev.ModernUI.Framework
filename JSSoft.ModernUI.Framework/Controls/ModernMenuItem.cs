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

using Ntreev.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Ntreev.ModernUI.Framework.Controls
{
    [ContentProperty(nameof(MenuItems))]
    [DefaultProperty(nameof(MenuItems))]
    public class ModernMenuItem : MenuItem
    {
        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register(nameof(DataType), typeof(Type), typeof(ModernMenuItem));

        private static readonly DependencyPropertyKey MenuItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MenuItems), typeof(IList), typeof(ModernMenuItem),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public new static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ModernMenuItem),
                new FrameworkPropertyMetadata(ItemsSourcePropertyChangedCallback));

        private readonly ObservableCollection<object> menuItems = new ObservableCollection<object>();

        static ModernMenuItem()
        {
            CommandProperty.OverrideMetadata(typeof(ModernMenuItem),
                new FrameworkPropertyMetadata(null, CommandPropertyChangedCallback, CommandPropertyCoerceValueCallback));
            CommandParameterProperty.OverrideMetadata(typeof(ModernMenuItem),
                new FrameworkPropertyMetadata(null, CommandParameterPropertyChangedCallback));
        }

        public ModernMenuItem()
        {
            this.SetValue(MenuItemsPropertyKey, this.menuItems);
        }

        public Type DataType
        {
            get => (Type)this.GetValue(DataTypeProperty);
            set => this.SetValue(DataTypeProperty, value);
        }

        public IList MenuItems
        {
            get => (IList)this.GetValue(MenuItemsProperty);
            private set => this.SetValue(MenuItemsPropertyKey, value);
        }

        public new IEnumerable ItemsSource
        {
            get => (IEnumerable)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.RefreshItemsSource(Enumerable.Empty<object>());
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is IMenuItem)
                return false;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ModernMenuItem();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (element is ModernMenuItem menuItem)
            {
                menuItem.ItemContainerStyleSelector = this.ItemContainerStyleSelector;
            }
        }

        protected override void OnPreviewMouseDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernMenuItem self)
            {
                self.RefreshItemsSource(e.NewValue as IEnumerable ?? new object[] { });
            }
        }

        private static object CommandPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            //if (self.contextMenu.IsOpen == false)
            //    return null;
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

        private void RefreshItemsSource(IEnumerable items)
        {
            var list = new List<object>();
            var menuItemByType = new Dictionary<Type, ModernMenuItem>(this.MenuItems.Count);
            foreach (var item in this.MenuItems)
            {
                if (item is ModernMenuItem menuItem && menuItem.DataType != null)
                {
                    menuItemByType.Add(menuItem.DataType, menuItem);
                }
                list.Add(item);
            }
            foreach (var item in items)
            {
                if (menuItemByType.ContainsKey(item.GetType()) == true)
                {
                    menuItemByType[item.GetType()].DataContext = item;
                }
                else
                {
                    list.Add(item);
                }
            }

            var categories = CategoryDefinitionAttribute.GetCategoryDefinitions(this.DataContext);
            var query = from item in list
                        group item by CategoryNameAttribute.GetCategory(item) into groupItem
                        orderby groupItem.Key
                        select groupItem;

            var itemList = new List<object>();
            var index = 0;
            foreach (var groupItem in CategoryDefinitionAttribute.Order(query, item => item.Key, categories))
            {
                if (index > 0)
                {
                    itemList.Add(new Separator());
                }
                foreach (var item in groupItem.OrderBy(i => OrderAttribute.GetOrder(i)))
                {
                    itemList.Add(item);
                }
                index++;
            }

            this.SetValue(ItemsControl.ItemsSourceProperty, itemList);
        }
    }
}
