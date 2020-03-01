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

namespace Ntreev.ModernUI.Framework.Controls
{
    public static class TreeViewUtility
    {
        private const string SelectedItemBindingPath = nameof(SelectedItemBindingPath);
        private const string SelectedItem = nameof(SelectedItem);

        public static readonly DependencyProperty SelectedItemBindingPathProperty =
            DependencyProperty.RegisterAttached(SelectedItemBindingPath, typeof(string), typeof(TreeViewUtility),
                new PropertyMetadata(null, SelectedItemBindingPathPropertyChangedCallback));

        private static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.RegisterAttached(SelectedItem, typeof(object), typeof(TreeViewUtility));

        public static string GetSelectedItemBindingPath(TreeView treeView)
        {
            return (string)treeView.GetValue(SelectedItemBindingPathProperty);
        }

        public static void SetSelectedItemBindingPath(TreeView treeView, object value)
        {
            treeView.SetValue(SelectedItemBindingPathProperty, value);
        }

        private static void SelectedItemBindingPathPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TreeView treeView)
            {
                treeView.Loaded -= TreeView_Loaded;
                treeView.Loaded += TreeView_Loaded;

                if (treeView.IsLoaded == true)
                {
                    OnTreeViewLoaded(treeView);
                }
            }
        }

        private static void TreeView_Loaded(object sender, RoutedEventArgs e)
        {
            OnTreeViewLoaded(sender as TreeView);
        }

        private static void OnTreeViewLoaded(TreeView treeView)
        {
            var path = (string)treeView.GetValue(SelectedItemBindingPathProperty);

            BindingOperations.SetBinding(treeView, TreeViewUtility.SelectedItemProperty, new Binding(path)
            {
                Source = treeView.DataContext,
                Mode = BindingMode.TwoWay,
            });

            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        private static void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var treeView = sender as TreeView;
            treeView.SetValue(TreeViewUtility.SelectedItemProperty, treeView.SelectedItem);
        }
    }
}
