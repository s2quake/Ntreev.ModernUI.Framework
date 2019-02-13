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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    public class ModernColumnManagerRow : ColumnManagerRow
    {
        private readonly static DependencyPropertyKey ColumnsWidthPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("ColumnsWidth", typeof(double), typeof(ModernColumnManagerRow),
                new UIPropertyMetadata(double.NaN));

        public readonly static DependencyProperty ColumnsWidthProperty = ColumnsWidthPropertyKey.DependencyProperty;

        private ScrollContentPresenter scrollContentPresenter;

        public ModernColumnManagerRow()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.scrollContentPresenter = FindParent<ScrollContentPresenter>(this);
        }

        public static double GetColumnsWidth(DependencyObject d)
        {
            return (double)d.GetValue(ColumnsWidthProperty);
        }

        protected override Cell CreateCell(ColumnBase column)
        {
            return new ModernColumnManagerCell();
        }

        protected override void PrepareContainer(DataGridContext dataGridContext, object item)
        {
            base.PrepareContainer(dataGridContext, item);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            var size = base.MeasureOverride(constraint);
            //this.Dispatcher.InvokeAsync(this.UpdateColumnsWidth, System.Windows.Threading.DispatcherPriority.Render);
            return size;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var size = base.ArrangeOverride(arrangeBounds);
            this.UpdateColumnsWidth();
            return size;
        }

        private void UpdateColumnsWidth()
        {
            if (DataGridControl.GetDataGridContext(this) is DataGridContext gridContext)
            {
                var scrollSize = this.scrollContentPresenter.DesiredSize;
                if (scrollSize.Width < this.DesiredSize.Width)
                {
                    gridContext.SetValue(ColumnsWidthPropertyKey, scrollSize.Width + 1);
                }
                else
                {
                    gridContext.SetValue(ColumnsWidthPropertyKey, this.DesiredSize.Width);
                }
            }
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null) return null;

            if (parentObject is T parent)
                return parent;
            else
                return FindParent<T>(parentObject);
        }
    }
}
