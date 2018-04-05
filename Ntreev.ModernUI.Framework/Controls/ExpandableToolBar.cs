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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace Ntreev.ModernUI.Framework.Controls
{
    [ContentProperty("MenuItems")]
    [DefaultProperty("MenuItems")]
    public class ExpandableToolBar : ToolBar
    {
        private static readonly DependencyPropertyKey MenuItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(MenuItems), typeof(IList), typeof(ExpandableToolBar),
                new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MenuItemsProperty = MenuItemsPropertyKey.DependencyProperty;

        public new static DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(ExpandableToolBar),
                new FrameworkPropertyMetadata(ItemsSourcePropertyChangedCallback));

        private readonly ObservableCollection<object> menuItems = new ObservableCollection<object>();

        public ExpandableToolBar()
        {
            this.SetValue(MenuItemsPropertyKey, this.menuItems);
            this.menuItems.CollectionChanged += MenuItems_CollectionChanged;
        }

        public IList MenuItems
        {
            get { return (IList)this.GetValue(MenuItemsProperty); }
            private set { this.SetValue(MenuItemsPropertyKey, value); }
        }

        public new IEnumerable ItemsSource
        {
            get { return (IEnumerable)this.GetValue(ItemsSourceProperty); }
            set { this.SetValue(ItemsSourceProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            this.RefreshItemsSource(Enumerable.Empty<object>());
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            if (item is IToolBarItem)
                return false;
            return base.IsItemItsOwnContainerOverride(item);
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ContentControl();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            //if (item is IToolBarItem && element is Button button)
            //{
            //    var be = BindingOperations.GetBindingExpression(button, Button.CommandProperty);
            //    be.UpdateTarget();

            //}
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);
        }

        private static void ItemsSourcePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ExpandableToolBar self)
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
            foreach (var item in this.MenuItems)
            {
                list.Add(item);
            }
            foreach (var item in items)
            {
                list.Add(item);
            }

            var query = from item in list
                        group item by this.GetCategory(item) into groupItem
                        orderby groupItem.Key
                        select groupItem;

            var itemList = new List<object>();
            var index = 0;
            var comparer = new Comparer();
            foreach (var groupItem in query.OrderBy(i => i.Key, comparer))
            {
                if (index > 0)
                {
                    itemList.Add(new Separator());
                }
                foreach (var item in groupItem.OrderBy(i => this.GetOrder(i)))
                {
                    itemList.Add(item);
                }
                index++;
            }

            this.SetValue(ItemsControl.ItemsSourceProperty, itemList);
        }

        private string GetCategory(object item)
        {
            if (item is Control)
            {
                return CategoryAttribute.Default.Category;
            }
            else if (Attribute.GetCustomAttribute(item.GetType(), typeof(DefaultMenuAttribute), false) is DefaultMenuAttribute menuAttr)
            {
                return CategoryAttribute.Default.Category;
            }
            else if (Attribute.GetCustomAttribute(item.GetType(), typeof(CategoryAttribute), false) is CategoryAttribute categoryAttr)
            {
                if (string.IsNullOrEmpty(categoryAttr.Category) == true)
                    return CategoryAttribute.Data.Category;
                return categoryAttr.Category;
            }
            return CategoryAttribute.Data.Category;
        }

        private int GetOrder(object item)
        {
            if (Attribute.GetCustomAttribute(item.GetType(), typeof(DefaultMenuAttribute), false) is DefaultMenuAttribute menuAttr)
            {
                return menuAttr.Order - 1;
            }
            else if (Attribute.GetCustomAttribute(item.GetType(), typeof(OrderAttribute), false) is OrderAttribute categoryAttr)
            {
                return categoryAttr.Order;
            }
            return 0;
        }

        #region classes

        class Comparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var x1 = x == CategoryAttribute.Default.Category ? string.Empty : x;
                var y1 = y == CategoryAttribute.Default.Category ? string.Empty : y;
                return x1.CompareTo(y1);
            }
        }

        #endregion
    }
}
