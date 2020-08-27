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
using JSSoft.Library.Linq;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public class ModernCurrentItemElement : FrameworkElement
    {
        private const int itemHeight = 2;

        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(ModernCurrentItemElement),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.Black)));

        public static readonly DependencyProperty ScrollBarProperty =
            DependencyProperty.Register(nameof(ScrollBar), typeof(ModernScrollBar), typeof(ModernCurrentItemElement),
                new FrameworkPropertyMetadata(null, ScrollBarPropertyChangedCallback));

        private int index = -1;
        private int count = 0;

        public ModernCurrentItemElement()
        {

        }

        public ModernScrollBar ScrollBar
        {
            get => (ModernScrollBar)this.GetValue(ScrollBarProperty);
            set => this.SetValue(ScrollBarProperty, value);
        }

        public Brush LineBrush
        {
            get => (Brush)this.GetValue(LineBrushProperty);
            set => this.SetValue(LineBrushProperty, value);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (this.index < 0 || this.count == 0)
                return;
            var rowHeight = this.ActualHeight / this.count;
            var y = (int)((rowHeight * this.index) + (rowHeight - itemHeight) * 0.5);
            dc.DrawRectangle(this.LineBrush, null, new Rect(new Point(0, y), new Point(this.ActualWidth, y + itemHeight)));
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            var gridControl = gridContext.DataGridControl;
            gridControl.PropertyChanged += GridControl_PropertyChanged;
            gridControl.ItemsSourceChangeCompleted += GridControl_ItemsSourceChangeCompleted;
        }

        private void GridControl_ItemsSourceChangeCompleted(object sender, EventArgs e)
        {
            this.RefreshCount();
            this.RefreshIndex();
        }

        private static void ScrollBarPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernCurrentItemElement self)
            {
                if (e.OldValue is ModernScrollBar oldScrollBar)
                {
                    oldScrollBar.RangeChanged -= self.ScrollBar_RangeChanged;
                }
                if (e.NewValue is ModernScrollBar newScrollBar)
                {
                    newScrollBar.RangeChanged += self.ScrollBar_RangeChanged;
                    self.RefreshCount();
                    self.RefreshIndex();
                }
            }
        }

        private void GridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(DataGridControl.GlobalCurrentItem))
            {
                this.RefreshIndex();
            }
        }

        private void ScrollBar_RangeChanged(object sender, EventArgs e)
        {
            this.RefreshCount();
            this.RefreshIndex();
        }

        private void RefreshCount()
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            if (gridContext != null)
            {
                this.count = gridContext.GetScrollableItems().Count();
                this.InvalidateVisual();
            }
        }

        private void RefreshIndex()
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(this);
            if (gridContext != null)
            {
                var gridControl = gridContext.DataGridControl;
                if (gridControl.GlobalCurrentItem == null)
                {
                    this.index = -1;
                }
                else
                {
                    this.index = EnumerableUtility.IndexOf(gridContext.GetScrollableItems(), gridControl.GlobalCurrentItem);
                }
                this.InvalidateVisual();
            }
        }
    }
}
