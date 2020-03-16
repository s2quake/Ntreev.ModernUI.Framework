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
using System.Windows.Input;
using System.Windows.Markup;

namespace Ntreev.ModernUI.Framework.Controls
{
    [ContentProperty(nameof(MenuItems))]
    [DefaultProperty(nameof(MenuItems))]
    public class ModernContextMenu : ContextMenu
    {
        private static readonly DependencyPropertyKey MenuItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MenuItems), typeof(IList), typeof(ModernContextMenu),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public new static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ModernContextMenu),
                new FrameworkPropertyMetadata(ItemsSourcePropertyChangedCallback));

        private readonly ObservableCollection<object> menuItems = new ObservableCollection<object>();

        static ModernContextMenu()
        {
            
        }

        public ModernContextMenu()
        {
            this.SetValue(MenuItemsPropertyKey, this.menuItems);
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
            return new ModernContextMenuItem(this);
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void OnOpened(RoutedEventArgs e)
        {
            base.OnOpened(e);

            var items = base.ItemsSource.OfType<object>();
            if (items.FirstOrDefault() is Separator s1)
            {
                s1.Visibility = Visibility.Collapsed;
            }

            var isSeparator = items.FirstOrDefault() is Separator;
            foreach (var item in base.ItemsSource)
            {
                var container = this.ItemContainerGenerator.ContainerFromItem(item);
                if (container is UIElement element && element.Visibility != Visibility.Visible)
                    continue;

                if (isSeparator == true && item is Separator s)
                {
                    s.Visibility = Visibility.Collapsed;
                }
                isSeparator = item is Separator;
            }

            if (items.LastOrDefault() is Separator s2)
            {
                s2.Visibility = Visibility.Collapsed;
            }
        }

        protected override void OnClosed(RoutedEventArgs e)
        {
            base.OnClosed(e);

            foreach (var item in base.ItemsSource)
            {
                if (item is Separator s)
                {
                    s.Visibility = Visibility.Visible;
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernContextMenu self)
            {
                self.RefreshItemsSource((e.NewValue as IEnumerable) ?? new object[] { });
            }
        }

        private void RefreshItemsSource(IEnumerable items)
        {
            var list = new List<object>();
            foreach (var item in this.MenuItems)
            {
                list.Add(item);
            }
            foreach (var item in items)
            {
                list.Add(item);
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
