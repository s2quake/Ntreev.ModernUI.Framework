// Released under the MIT License.
// 
// Copyright (c) 2018 Ntreev Soft co., Ltd.
// Copyright (c) 2020 Jeesu Choi
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation the
// rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit
// persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
// Forked from https://github.com/NtreevSoft/Ntreev.ModernUI.Framework
// Namespaces and files starting with "Ntreev" have been renamed to "JSSoft".

using JSSoft.Library;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;

namespace JSSoft.ModernUI.Framework.Controls
{
    [ContentProperty(nameof(MenuItems))]
    [DefaultProperty(nameof(MenuItems))]
    public class ModernMenu : Menu
    {
        private static readonly DependencyPropertyKey MenuItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MenuItems), typeof(IList), typeof(ModernMenu),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public new static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ModernMenu),
                new FrameworkPropertyMetadata(ItemsSourcePropertyChangedCallback));

        private readonly ObservableCollection<object> menuItems = new();

        public ModernMenu()
        {
            this.SetValue(MenuItemsPropertyKey, this.menuItems);
            this.menuItems.CollectionChanged += MenuItems_CollectionChanged;
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

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernMenu self)
            {
                self.RefreshItemsSource(e.NewValue as IEnumerable);
            }
        }

        private void MenuItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

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
            foreach (var groupItem in CategoryDefinitionAttribute.Order(query, item => item.Key, categories))
            {
                foreach (var item in groupItem.OrderBy(i => OrderAttribute.GetOrder(i)))
                {
                    itemList.Add(item);
                }
            }

            this.SetValue(ItemsControl.ItemsSourceProperty, itemList);
        }
    }
}
