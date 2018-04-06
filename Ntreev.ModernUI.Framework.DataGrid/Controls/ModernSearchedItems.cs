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
using System.Windows.Media;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.Controls
{
    class ModernSearchedItems : FrameworkElement
    {
        private const int itemHeight = 6;

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.RegisterAttached(nameof(LineBrush), typeof(Brush), typeof(ModernSearchedItems),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ScrollBarProperty =
            DependencyProperty.Register(nameof(ScrollBar), typeof(ModernScrollBar), typeof(ModernSearchedItems),
                new FrameworkPropertyMetadata(null, ScrollBarPropertyChangedCallback));

        private readonly ObservableCollection<int> filteredItems = new ObservableCollection<int>();
        private int count;

        public ModernSearchedItems()
        {

        }

        public ModernScrollBar ScrollBar
        {
            get { return (ModernScrollBar)this.GetValue(ScrollBarProperty); }
            set { this.SetValue(ScrollBarProperty, value); }
        }

        public Brush LineBrush
        {
            get { return (Brush)this.GetValue(LineBrushProperty); }
            set { this.SetValue(LineBrushProperty, value); }
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (this.filteredItems.Any() == false)
                return;

            var gridContext = Xceed.Wpf.DataGrid.DataGridControl.GetDataGridContext(this);
            var pen = new Pen(this.LineBrush, 1);

            var rowHeight = this.ActualHeight / this.count;
            var rowWidth = (int)(this.ActualWidth * 0.4);

            foreach (var item in this.filteredItems)
            {
                var y = (int)((rowHeight * item) + (rowHeight - itemHeight) * 0.5);
                dc.DrawRectangle(this.LineBrush, null, new Rect(new Point(this.ActualWidth - rowWidth, y), new Point(this.ActualWidth, y + itemHeight)));
            }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl;
            gridControl.PropertyChanged += GridControl_PropertyChanged;
        }

        private static void ItemsPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as ModernSearchedItems;
            if (e.NewValue is INotifyCollectionChanged == true)
            {
                (e.NewValue as INotifyCollectionChanged).CollectionChanged += self.ModernFilteredItems_CollectionChanged;
            }

            self.InvalidateVisual();
        }

        private void ModernFilteredItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.InvalidateVisual();
        }

        private static void ScrollBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernSearchedItems self)
            {
                if (e.OldValue is ModernScrollBar oldScrollBar)
                {
                    oldScrollBar.RangeChanged -= self.ScrollBar_RangeChanged;
                }
                if (e.NewValue is ModernScrollBar newScrollBar)
                {
                    newScrollBar.RangeChanged += self.ScrollBar_RangeChanged;
                    self.Refresh();
                }
            }
        }

        private void GridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ModernDataGridControl.SearchText))
            {
                this.Refresh();
            }
        }

        private void ScrollBar_RangeChanged(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private void Refresh()
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            if (gridContext != null)
            {
                var gridControl = gridContext.DataGridControl as ModernDataGridControl;
                this.filteredItems.Clear();

                if (gridControl.SearchText != string.Empty)
                {
                    var i = 0;
                    var items = gridContext.GetScrollableItemInfos();
                    foreach (var item in items)
                    {
                        if (gridControl.Match(item.GridContext, item.Item) != null)
                        {
                            this.filteredItems.Add(i);
                        }
                        i++;
                    }
                    this.count = items.Count();
                }
                this.InvalidateVisual();
            }
        }
    }
}
