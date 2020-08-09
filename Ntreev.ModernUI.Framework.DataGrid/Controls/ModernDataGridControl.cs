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

using Ntreev.Library.Linq;
using Ntreev.ModernUI.Framework.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Xceed.Wpf.DataGrid;
using Xceed.Wpf.DataGrid.Views;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    /// <summary>
    /// ModernDataGridControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public class ModernDataGridControl : DataGridControl
    {
        private const string contentToStringConverterString = "contentToStringConverter";

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(string.Empty, FilterPropertyChangedCallback, FilterCoerceValueCallback));

        private static readonly DependencyPropertyKey HasSearchTextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(HasSearchText), typeof(bool), typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasSearchTextProperty = HasSearchTextPropertyKey.DependencyProperty;

        public static readonly DependencyProperty IsVerticalScrollBarOnLeftSideProperty =
            DependencyProperty.Register("IsVerticalScrollBarOnLeftSide", typeof(bool), typeof(ModernDataGridControl));

        public static readonly DependencyProperty AllowHeightResizeProperty =
            DependencyProperty.RegisterAttached("AllowHeightResize", typeof(bool), typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty GroupIndexProperty =
            DependencyProperty.RegisterAttached("GroupIndex", typeof(int), typeof(ModernDataGridControl), new PropertyMetadata(0));

        public static readonly DependencyProperty AllowRowDragProperty =
            DependencyProperty.Register(nameof(AllowRowDrag), typeof(bool), typeof(ModernDataGridControl), new PropertyMetadata(false));

        private static readonly DependencyPropertyKey HasDataContextErrorPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly("HasDataContextError", typeof(bool), typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(false));
        public static readonly DependencyProperty HasDataContextErrorProperty = HasDataContextErrorPropertyKey.DependencyProperty;

        public static readonly DependencyProperty DataContextErrorProperty =
            DependencyProperty.RegisterAttached("DataContextError", typeof(object), typeof(ModernDataGridControl),
                new FrameworkPropertyMetadata(null, DataContextErrorPropertyChangedCallback, DataContextErrorCoerceValueCallback));

        public static readonly DependencyProperty IsCurrentContextProperty =
            DependencyProperty.RegisterAttached("IsCurrentContext", typeof(bool), typeof(ModernDataGridControl),
                new PropertyMetadata(false));

        public static readonly RoutedEvent ValueChangingEvent =
            EventManager.RegisterRoutedEvent(nameof(ValueChanging), RoutingStrategy.Bubble,
                typeof(ValueChangingEventHandler), typeof(ModernDataGridControl));

        public static readonly RoutedEvent ItemDeletingEvent =
            EventManager.RegisterRoutedEvent(nameof(ItemDeleting), RoutingStrategy.Bubble,
                typeof(ItemDeletingEventHandler), typeof(ModernDataGridControl));

        public static readonly RoutedEvent RowDragEvent =
            EventManager.RegisterRoutedEvent(nameof(RowDrag), RoutingStrategy.Bubble,
                typeof(ModernDragEventHandler), typeof(ModernDataGridControl));

        public static readonly RoutedEvent RowDropEvent =
            EventManager.RegisterRoutedEvent(nameof(RowDrop), RoutingStrategy.Bubble,
                typeof(ModernDragEventHandler), typeof(ModernDataGridControl));

        private static readonly List<Color> colors = new List<Color>();

        private DataGridContext selectedGridContext;
        private SelectionRange selectedColumnRange;
        private SelectionRange selectedItemRange;
        private DataGridContext currentContext;

        private static Point dragPoint;
        private static RowSelector dragSelector;
        private static DataGridContext dragContext;
        private static ScrollViewer dragViewer;
        private static bool isDragDrop;
        private static int scrollDirection;
        private IValueConverter contentToStringConverter;

        static ModernDataGridControl()
        {
            colors.Add(Color.FromRgb(125, 102, 8));
            colors.Add(Color.FromRgb(20, 90, 50));
            colors.Add(Color.FromRgb(120, 66, 18));
            colors.Add(Color.FromRgb(0x00, 0xab, 0xa9));
            colors.Add(Color.FromRgb(0x1b, 0xa1, 0xe2));
            colors.Add(Color.FromRgb(0x00, 0x50, 0xef));
            colors.Add(Color.FromRgb(0x6a, 0x00, 0xff));
            colors.Add(Color.FromRgb(0xaa, 0x00, 0xff));
            colors.Add(Color.FromRgb(0xf4, 0x72, 0xd0));
            colors.Add(Color.FromRgb(0xd8, 0x00, 0x73));
            colors.Add(Color.FromRgb(0xa2, 0x00, 0x25));
            colors.Add(Color.FromRgb(0xe5, 0x14, 0x00));
            colors.Add(Color.FromRgb(0xfa, 0x68, 0x00));
            colors.Add(Color.FromRgb(0xf0, 0xa3, 0x0a));
            colors.Add(Color.FromRgb(0xe3, 0xc8, 0x00));
            colors.Add(Color.FromRgb(0x82, 0x5a, 0x2c));
            colors.Add(Color.FromRgb(0x6d, 0x87, 0x64));
            colors.Add(Color.FromRgb(0x64, 0x76, 0x87));
            colors.Add(Color.FromRgb(0x76, 0x60, 0x8a));
            colors.Add(Color.FromRgb(0x87, 0x79, 0x4e));

            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(RowSelector_PreviewMouseLeftButtonDown), true);
            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.PreviewMouseMoveEvent, new MouseEventHandler(RowSelector_PreviewMouseMove), true);
            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.DropEvent, new DragEventHandler(RowSelector_Drop), true);
            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.DragEnterEvent, new DragEventHandler(RowSelector_DragEnter), true);
            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.DragOverEvent, new DragEventHandler(RowSelector_DragOver), true);
            EventManager.RegisterClassHandler(typeof(RowSelector), UIElement.DragLeaveEvent, new DragEventHandler(RowSelector_DragLeave), true);
        }

        public ModernDataGridControl()
        {
            this.AllowDrop = true;

            (this.GroupLevelDescriptions as INotifyCollectionChanged).CollectionChanged += GroupLevelDescriptionCollection_CollectionChanged;
            this.UpdateSourceTrigger = DataGridUpdateSourceTrigger.CellContentChanged;

            this.AddEditor(typeof(string));
            this.AddEditor(typeof(bool));
            this.AddEditor(typeof(byte));
            this.AddEditor(typeof(sbyte));
            this.AddEditor(typeof(short));
            this.AddEditor(typeof(ushort));
            this.AddEditor(typeof(int));
            this.AddEditor(typeof(uint));
            this.AddEditor(typeof(long));
            this.AddEditor(typeof(ulong));
            this.AddEditor(typeof(float));
            this.AddEditor(typeof(double));
            this.AddEditor(typeof(TimeSpan));
            this.AddEditor(typeof(DateTime));

            this.CommandBindings.Add(new CommandBinding(ModernDataGridCommands.CopyWithHeaders, this.OnCopyWithHeadersExecuted));
            this.CommandBindings.Add(new CommandBinding(ModernDataGridCommands.NextMatchedItem, NextMatchedItem_Execute, NextMatchedItem_CanExecute));
            this.CommandBindings.Add(new CommandBinding(ModernDataGridCommands.PrevMatchedItem, PrevMatchedItem_Execute, PrevMatchedItem_CanExecute));

            this.ClipboardExporters.Clear();
            this.ClipboardExporters.Add(DataFormats.UnicodeText, new ModernTextClipboardExporter());

            this.PropertyChanged += ModernDataGridControl_PropertyChanged;
            if (this.CurrentContext != null)
            {
                SetIsCurrentContext(this.CurrentContext, true);
            }
        }

        public static int GetGroupIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(GroupIndexProperty);
        }

        public static void SetGroupIndex(DependencyObject obj, int value)
        {
            obj.SetValue(GroupIndexProperty, value);
        }

        public static bool GetHasDataContextError(DependencyObject d)
        {
            return (bool)d.GetValue(HasDataContextErrorProperty);
        }

        public static object GetDataContextError(DependencyObject d)
        {
            return (object)d.GetValue(DataContextErrorProperty);
        }

        public static void SetDataContextError(DependencyObject d, object value)
        {
            d.SetValue(DataContextErrorProperty, value);
        }

        public static bool GetIsCurrentContext(DependencyObject d)
        {
            return (bool)d.GetValue(IsCurrentContextProperty);
        }

        public static void SetIsCurrentContext(DependencyObject d, bool value)
        {
            d.SetValue(IsCurrentContextProperty, value);
        }

        public event ValueChangingEventHandler ValueChanging
        {
            add { AddHandler(ValueChangingEvent, value); }
            remove { RemoveHandler(ValueChangingEvent, value); }
        }

        public event ItemDeletingEventHandler ItemDeleting
        {
            add { AddHandler(ItemDeletingEvent, value); }
            remove { RemoveHandler(ItemDeletingEvent, value); }
        }

        public event ModernDragEventHandler RowDrag
        {
            add { AddHandler(RowDragEvent, value); }
            remove { RemoveHandler(RowDragEvent, value); }
        }

        public event ModernDragEventHandler RowDrop
        {
            add { AddHandler(RowDropEvent, value); }
            remove { RemoveHandler(RowDropEvent, value); }
        }

        public void AdjustColumnsWidth()
        {
            this.UpdateLayout();
            foreach (var item in this.Columns)
            {
                var width = item.GetFittedWidth();
                if (width < 0)
                    continue;
                item.Width = width;
            }
        }

        public static bool GetAllowHeightResize(Row row)
        {
            return (bool)row.GetValue(AllowHeightResizeProperty);
        }

        public static void SetAllowHeightResize(Row row, bool value)
        {
            row.SetValue(AllowHeightResizeProperty, value);
        }

        public static Color GetColor(int index)
        {
            if (colors.Any() == false || index < 0)
            {
                return Colors.White;
            }

            return colors[index % colors.Count];
        }

        public void PrevMatchedItem()
        {
            this.PrevMatchedItem(false);
        }

        public void NextMatchedItem()
        {
            this.NextMatchedItem(false);
        }

        public SelectedFlags SelectedFlags { get; private set; }

        public string SearchText
        {
            get => (string)this.GetValue(SearchTextProperty);
            set => this.SetValue(SearchTextProperty, value);
        }

        public bool HasSearchText => (bool)this.GetValue(HasSearchTextProperty);

        public bool IsVerticalScrollBarOnLeftSide
        {
            get => (bool)this.GetValue(IsVerticalScrollBarOnLeftSideProperty);
            set => this.SetValue(IsVerticalScrollBarOnLeftSideProperty, value);
        }

        public bool AllowRowDrag
        {
            get => (bool)this.GetValue(AllowRowDragProperty);
            set => this.SetValue(AllowRowDragProperty, value);
        }

        public void Select(DataGridContext gridContext, object item)
        {
            this.ClearSelection();
            this.SelectedFlags = SelectedFlags.Row;
            this.selectedItemRange = new SelectionRange(gridContext.Items.IndexOf(item));
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange.StartIndex, 0, this.selectedItemRange.EndIndex, gridContext.VisibleColumns.Count - 1));
            this.selectedGridContext.CurrentItem = item;
            if (this.selectedGridContext.CurrentColumn == null && this.selectedGridContext.VisibleColumns.Count > 0)
                this.selectedGridContext.CurrentColumn = this.selectedGridContext.VisibleColumns[0];
            this.selectedGridContext.FocusCurrent();
        }

        public void SelectTo(DataGridContext gridContext, object item)
        {
            if (this.selectedGridContext != gridContext)
                return;

            this.ClearSelection();
            this.SelectionUnit = SelectionUnit.Cell;
            this.SelectedFlags = SelectedFlags.Row | Controls.SelectedFlags.Multiple;
            this.selectedItemRange.EndIndex = gridContext.Items.IndexOf(item);
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange.StartIndex, 0, this.selectedItemRange.EndIndex, gridContext.VisibleColumns.Count - 1));
            this.selectedGridContext.CurrentItem = item;
        }

        public void AddSelection(DataGridContext gridContext, object item)
        {
            if (this.selectedGridContext != gridContext)
                return;

            this.SelectedFlags |= (SelectedFlags.Row | SelectedFlags.Multiple);
            this.selectedItemRange = new SelectionRange(gridContext.Items.IndexOf(item));
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange.StartIndex, 0, this.selectedItemRange.EndIndex, gridContext.VisibleColumns.Count - 1));
            this.selectedGridContext.CurrentItem = item;
        }

        public override void OnApplyTemplate()
        {
            if (DesignerProperties.GetIsInDesignMode(this) == true)
            {
                if (this.View == null)
                {
                    this.View = new TableView();
                }
            }
            base.OnApplyTemplate();
        }

        protected override void OnDeleteExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            base.OnDeleteExecuted(sender, e);
        }

        protected override void OnSelectionChanged(DataGridSelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is ModernDataRow;
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new ModernDataRow();
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.None)
            {
                if (e.Key == Key.Left)
                {
                    var gridContext = this.CurrentContext;
                    if (gridContext != null && gridContext.CurrentColumn != null)
                    {
                        if (gridContext.VisibleColumns.First() == gridContext.CurrentColumn && this.IsBeingEdited == false)
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                }
                else if (e.Key == Key.Right)
                {
                    var gridContext = this.CurrentContext;
                    if (gridContext != null && gridContext.CurrentColumn != null)
                    {
                        if (gridContext.VisibleColumns.Last() == gridContext.CurrentColumn && this.IsBeingEdited == false)
                        {
                            e.Handled = true;
                            return;
                        }
                    }
                }
            }

            base.OnPreviewKeyDown(e);
        }

        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            base.OnPreviewDragEnter(e);
        }

        protected override void OnPreviewGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if (e.NewFocus is ModernDataCell)
                {
                    //var cell = e.NewFocus as ModernDataCell;
                    //if (cell.IsFocused == false)
                    //{
                    //    if (e.KeyboardDevice.Modifiers == ModifierKeys.Control)
                    //    {
                    //        this.AddSelection(cell);
                    //    }
                    //    else
                    //    {
                    //        this.Select(cell);
                    //    }
                    //}
                    //e.Handled = true;
                }
                //e.Handled = true;
                base.OnPreviewGotKeyboardFocus(e);
            }
            catch (ModernDataGridUpdateSourceException ex)
            {
                AppMessageBox.ShowErrorAsync(ex.Message);
                ex.Cell.CancelEdit();
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
        }

        protected virtual void OnPreviewItemDeleting(ItemDeletingEventArgs e)
        {


        }

        protected override void OnCopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var exporter = this.ClipboardExporters[DataFormats.UnicodeText];
            if (exporter != null)
                exporter.IncludeColumnHeaders = false;
            base.OnCopyExecuted(sender, e);
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected virtual void OnCopyWithHeadersExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var exporter = this.ClipboardExporters[DataFormats.UnicodeText];
            if (exporter != null)
                exporter.IncludeColumnHeaders = true;
            base.OnCopyExecuted(sender, e);
        }

        protected virtual void OnRowDrag(ModernDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual void OnRowDrop(ModernDragEventArgs e)
        {
            this.RaiseEvent(e);
        }

        private void AddEditor(Type type)
        {
            if (this.TryFindResource(type.FullName) is CellEditor editor)
                this.DefaultCellEditors.Add(type, editor);
        }

        //private void FocusDefault()
        //{
        //    if (this.GlobalCurrentItem == null)
        //        this.selectedGridContext.CurrentItem = this.selectedGridContext.Items.GetItemAt(0);
        //    if (this.GlobalCurrentColumn == null)
        //        this.selectedGridContext.CurrentColumn = this.selectedGridContext.VisibleColumns.FirstOrDefault();
        //}

        private void GroupLevelDescriptionCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as GroupLevelDescriptionCollection;

            int index = 0;
            foreach (var item in collection)
            {
                item.SetValue(ModernDataGridControl.GroupIndexProperty, index++);
            }
        }

        private static void RowSelector_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is RowSelector rowSelector && rowSelector.DataContext is ModernDataRow dataRow)
            {
                if (rowSelector.Template.FindName("PART_RowResizerThumb", rowSelector) is Thumb thumb && thumb.IsMouseOver == true)
                {
                    return;
                }
                var gridContext = DataGridControl.GetDataGridContext(dataRow);
                if (gridContext.DataGridControl is ModernDataGridControl gridControl)
                {
                    if (gridControl.ReadOnly == true || gridControl.AllowRowDrag == false)
                        return;

                    dragSelector = rowSelector;
                    dragContext = gridContext;
                    dragPoint = e.GetPosition(null);
                }
            }
        }

        private static void RowSelector_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released || dragSelector != sender || isDragDrop == true)
                return;

            var mousePos = e.GetPosition(null);
            var diff = dragPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                isDragDrop = true;
                var selector = sender as RowSelector;
                if (selector.DataContext is ModernDataRow)
                {
                    var dataRow = selector.DataContext as ModernDataRow;
                    var dataContext = dataRow.DataContext;
                    var dragData = new DataObject(sender);
                    var gridContext = DataGridControl.GetDataGridContext(selector);
                    var gridControl = gridContext.DataGridControl as ModernDataGridControl;
                    var scrollViewer = gridControl.Template.FindName("PART_ScrollViewer", gridControl) as ScrollViewer;


                    dragViewer = scrollViewer;
                    scrollViewer.DragEnter += ScrollViewer_DragEnter;
                    scrollViewer.DragOver += ScrollViewer_DragOver;
                    var timer = new Timer(100);
                    timer.Elapsed += async (ss, ee) =>
                    {
                        await scrollViewer.Dispatcher.InvokeAsync(() =>
                        {
                            if (scrollDirection == -1)
                                scrollViewer.LineUp();
                            else if (scrollDirection == 1)
                                scrollViewer.LineDown();
                        });
                    };
                    timer.Start();
                    var effects = DragDrop.DoDragDrop(sender as DependencyObject, dragData, DragDropEffects.Move | DragDropEffects.None);
                    timer.Stop();
                    scrollViewer.DragOver -= ScrollViewer_DragOver;
                    scrollViewer.DragEnter -= ScrollViewer_DragEnter;

                    if (effects == DragDropEffects.Move)
                    {
                        try
                        {
                            var args = new ModernDragEventArgs(dragData, dataRow.DataContext, gridContext)
                            {
                                RoutedEvent = ModernDataGridControl.RowDropEvent,
                                Source = gridControl,
                            };
                            gridControl.OnRowDrop(args);
                        }
                        catch (Exception ex)
                        {
                            AppMessageBox.ShowErrorAsync(ex);
                        }
                    }
                }
                isDragDrop = false;
            }
        }

        private static void ScrollViewer_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private static void ScrollViewer_DragOver(object sender, DragEventArgs e)
        {
            var scrollViewer = sender as ScrollViewer;
            ComputeScrollDirection(scrollViewer, e);
            e.Effects = DragDropEffects.None;
            e.Handled = true;
        }

        private static void ComputeScrollDirection(ScrollViewer scrollViewer, DragEventArgs e)
        {
            var contentPresenter = scrollViewer.Template.FindName("PART_ScrollContentPresenter", scrollViewer) as ScrollContentPresenter;
            var position = e.GetPosition(contentPresenter);

            if (position.Y < 10)
            {
                scrollDirection = -1;
            }
            else if (position.Y > contentPresenter.ActualHeight - 10)
            {
                scrollDirection = 1;
            }
            else
            {
                scrollDirection = 0;
            }
        }

        private static void RowSelector_Drop(object sender, DragEventArgs e)
        {
            var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
            if (dragSelector == selector)
                return;

            var dataRow = selector.DataContext as ModernDataRow;
            var gridContext = DataGridControl.GetDataGridContext(dataRow);
            var gridControl = gridContext.DataGridControl as ModernDataGridControl;
            var args = new ModernDragEventArgs(e.Data, dataRow.DataContext, gridContext)
            {
                RoutedEvent = ModernDataGridControl.RowDragEvent,
                Source = gridControl,
            };
            gridControl.OnRowDrag(args);
            dataRow.IsDragOver = false;
            ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
            e.Handled = true;
        }

        private static void RowSelector_DragEnter(object sender, DragEventArgs e)
        {
            var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
            if (dragSelector == selector)
                return;

            if (selector.DataContext is ModernDataRow dataRow && DataGridControl.GetDataGridContext(dataRow) == dragContext)
            {
                dataRow.IsDragOver = true;
                ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }
        }

        private static void RowSelector_DragOver(object sender, DragEventArgs e)
        {
            var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
            if (dragSelector == selector)
                return;

            if (selector.DataContext is ModernDataRow dataRow && DataGridControl.GetDataGridContext(dataRow) == dragContext)
            {
                e.Effects = DragDropEffects.Move;
                e.Handled = true;
            }
            else
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
            }

            ComputeScrollDirection(dragViewer, e);
        }

        private static void RowSelector_DragLeave(object sender, DragEventArgs e)
        {
            var selector = sender as Xceed.Wpf.DataGrid.RowSelector;
            if (dragSelector == selector)
                return;

            if (selector.DataContext is ModernDataRow dataRow && DataGridControl.GetDataGridContext(dataRow) == dragContext)
            {
                dataRow.IsDragOver = false;
                ModernDataRow.SetIsDragOver(selector, dataRow.IsDragOver);
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private static void FilterPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernDataGridControl self)
            {
                var filter = (string)e.NewValue;
                d.SetValue(HasSearchTextPropertyKey, filter != string.Empty);

                if (filter != string.Empty)
                {
                    if (self.GlobalCurrentItem != null && self.GlobalCurrentColumn != null)
                    {
                        if (self.Match(self.CurrentContext, self.GlobalCurrentItem, self.GlobalCurrentColumn.FieldName) == false)
                        {
                            self.NextMatchedItem();
                        }
                    }
                    else if (self.GlobalCurrentItem == null)
                    {
                        self.NextMatchedItem();
                    }
                }

                self.OnNotifyPropertyChanged(new PropertyChangedEventArgs(nameof(self.HasSearchText)));
                self.OnNotifyPropertyChanged(new PropertyChangedEventArgs(nameof(self.SearchText)));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private static object FilterCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return string.Empty;
            return baseValue;
        }

        private void NextMatchedItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.NextMatchedItem();
        }

        private void NextMatchedItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SearchText != string.Empty)
            {
                e.CanExecute = this.NextMatchedItem(true);
            }
        }

        private void PrevMatchedItem_Execute(object sender, ExecutedRoutedEventArgs e)
        {
            this.PrevMatchedItem();
        }

        private void PrevMatchedItem_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (this.SearchText != string.Empty)
            {
                e.CanExecute = this.PrevMatchedItem(true);
            }
        }

        private static void DataContextErrorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(HasDataContextErrorPropertyKey, e.NewValue != null);
        }

        private static object DataContextErrorCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue is string text && text == string.Empty)
                return null;
            return baseValue;
        }

        private void ModernDataGridControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(this.CurrentContext))
            {
                if (this.currentContext != null)
                {
                    SetIsCurrentContext(this.currentContext, this.currentContext.IsCurrent);
                }
                this.currentContext = this.CurrentContext;
                if (this.currentContext != null)
                {
                    SetIsCurrentContext(this.currentContext, this.currentContext.IsCurrent);
                }
            }
            else if (e.PropertyName == nameof(this.GlobalCurrentItem) || e.PropertyName == nameof(this.GlobalCurrentColumn))
            {
                if (this.SearchText != string.Empty)
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        private bool PrevMatchedItem(bool isVerify)
        {
            var scrollableItems = DataGridControl.GetDataGridContext(this).GetScrollableItemInfos().Reverse();
            var index = EnumerableUtility.IndexOf(scrollableItems, item => item.Item == this.GlobalCurrentItem);
            var items = index != -1 ? scrollableItems.Skip(index) : scrollableItems;
            foreach (var item in items)
            {
                if (item.Item is ICustomTypeDescriptor == false)
                    continue;

                var gridContext = item.GridContext;
                var dataContext = item.Item;
                var columnIndex = gridContext.VisibleColumns.Count - 1;
                if (this.GlobalCurrentItem == dataContext)
                {
                    columnIndex = gridContext.CurrentColumn == null ? gridContext.VisibleColumns.Count - 1 : gridContext.VisibleColumns.IndexOf(gridContext.CurrentColumn);
                    if (gridContext.CurrentColumn != null && columnIndex != 0)
                        columnIndex--;
                }

                for (var x = columnIndex; x >= 0; x--)
                {
                    var column = gridContext.VisibleColumns[x];
                    if (this.Match(gridContext, dataContext, column.FieldName) == true)
                    {
                        if (isVerify == false)
                        {
                            gridContext.CurrentItem = dataContext;
                            gridContext.CurrentColumn = column;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        private bool NextMatchedItem(bool isVerify)
        {
            var scrollableItems = DataGridControl.GetDataGridContext(this).GetScrollableItemInfos();
            var index = EnumerableUtility.IndexOf(scrollableItems, item => item.Item == this.GlobalCurrentItem);
            var items = index != -1 ? scrollableItems.Skip(index) : scrollableItems;
            foreach (var item in items)
            {
                if (item.Item is ICustomTypeDescriptor == false)
                    continue;

                var gridContext = item.GridContext;
                var dataContext = item.Item;
                var columnIndex = 0;
                if (this.GlobalCurrentItem == dataContext)
                {
                    columnIndex = gridContext.CurrentColumn == null ? 0 : gridContext.VisibleColumns.IndexOf(gridContext.CurrentColumn);
                    if (gridContext.CurrentColumn != null)
                        columnIndex++;
                }

                for (var x = columnIndex; x < gridContext.VisibleColumns.Count; x++)
                {
                    var column = gridContext.VisibleColumns[x];
                    if (this.Match(gridContext, dataContext, column.FieldName) == true)
                    {
                        if (isVerify == false)
                        {
                            gridContext.CurrentItem = dataContext;
                            gridContext.CurrentColumn = column;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        internal string Match(ModernItemInfo itemInfo)
        {
            return this.Match(itemInfo.GridContext, itemInfo.Item);
        }

        internal string Match(DataGridContext gridContext, object item)
        {
            if (item is ICustomTypeDescriptor)
            {
                foreach (var column in gridContext.VisibleColumns)
                {
                    if (this.Match(gridContext, item, column.FieldName) == true)
                        return column.FieldName;
                }
            }

            return null;
        }

        internal bool Match(DataGridContext _, object item, string fieldName)
        {
            if (this.contentToStringConverter == null)
            {
                this.contentToStringConverter = this.FindResource(contentToStringConverterString) as IValueConverter ?? new ContentToStringConverter();
            }

            if (item is ICustomTypeDescriptor descriptor)
            {
                var property = descriptor.GetProperties()[fieldName];
                if (property == null)
                    return false;
                var field = property.GetValue(item);
                if (field == DBNull.Value || field == null)
                    return false;
                if ($"{this.contentToStringConverter.Convert(field, typeof(string), null, CultureInfo.CurrentUICulture)}".IndexOf(this.SearchText, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    return true;
            }

            return false;
        }

        internal void SelectAll(DataGridContext gridContext)
        {
            if (this.SelectionUnit == SelectionUnit.Cell)
            {
                if (this.selectedGridContext != null)
                {
                    this.SelectedFlags = SelectedFlags.None;
                    this.selectedGridContext.SelectedCellRanges.Clear();
                    this.selectedGridContext.SelectedItemRanges.Clear();
                }

                this.selectedGridContext = gridContext;

                if (this.selectedGridContext.Items.Count > 0)
                {
                    this.SelectedFlags = SelectedFlags.All;
                    this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(0, 0, this.selectedGridContext.Items.Count - 1, this.selectedGridContext.Columns.Count - 1));
                    if (this.selectedGridContext.CurrentItem == null)
                        this.selectedGridContext.CurrentItem = this.selectedGridContext.Items.GetItemAt(0);
                    if (this.selectedGridContext.CurrentColumn == null && this.selectedGridContext.VisibleColumns.Count > 0)
                        this.selectedGridContext.CurrentColumn = this.selectedGridContext.VisibleColumns[0];

                    this.selectedGridContext.FocusCurrent();
                }
            }
            else
            {

            }
        }

        internal void Select(ModernColumnManagerCell column)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(column);
            var index = column.ParentColumn.VisiblePosition;

            this.ClearSelection();
            this.SelectedFlags = SelectedFlags.Column;
            this.selectedItemRange = new SelectionRange(gridContext.CurrentItemIndex);
            this.selectedColumnRange = new SelectionRange(index);
            this.selectedGridContext = gridContext;
            this.selectedGridContext.CurrentColumn = column.ParentColumn;
            if (gridContext.Items.Count > 0)
            {
                this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(0, this.selectedColumnRange.StartIndex, gridContext.Items.Count - 1, this.selectedColumnRange.EndIndex));
                if (this.selectedGridContext.CurrentItem == null && this.selectedGridContext.Items.Count > 0)
                    this.selectedGridContext.CurrentItem = this.selectedGridContext.Items.GetItemAt(0);
                this.selectedGridContext.FocusCurrent();
            }
        }

        internal void SelectTo(ModernColumnManagerCell column)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(column);
            if (this.selectedGridContext != gridContext)
                return;

            this.ClearSelection();
            this.SelectedFlags = SelectedFlags.Column;
            this.selectedColumnRange.EndIndex = column.ParentColumn.VisiblePosition;
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(0, this.selectedColumnRange.StartIndex, gridContext.Items.Count - 1, this.selectedColumnRange.EndIndex));
            this.selectedGridContext.CurrentColumn = column.ParentColumn;
        }

        internal void AddSelection(ModernColumnManagerCell column)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(column);
            if (this.selectedGridContext != gridContext)
                return;

            this.SelectedFlags |= SelectedFlags.Column;
            this.NavigationBehavior = NavigationBehavior.None;
            var index = column.ParentColumn.VisiblePosition;
            this.selectedColumnRange = new SelectionRange(index);
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(0, this.selectedColumnRange.StartIndex, gridContext.Items.Count - 1, this.selectedColumnRange.EndIndex));
            this.selectedGridContext.CurrentColumn = column.ParentColumn;
            this.NavigationBehavior = NavigationBehavior.CellOnly;
        }

        internal void AddSelectionTo(ModernColumnManagerCell column)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(column);
            if (this.selectedGridContext != gridContext)
                return;

            this.SelectedFlags |= SelectedFlags.Column;
            this.SelectionUnit = SelectionUnit.Cell;
            this.selectedColumnRange.EndIndex = column.ParentColumn.VisiblePosition;
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(0, this.selectedColumnRange.StartIndex, gridContext.Items.Count - 1, this.selectedColumnRange.EndIndex));
            this.selectedGridContext.CurrentColumn = column.ParentColumn;
        }

        internal void SelectTo(ModernDataCell cell)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(cell);
            if (this.selectedGridContext != gridContext)
                return;

            this.NavigationBehavior = NavigationBehavior.None;
            var column = gridContext.Columns[cell.FieldName];
            var row = cell.DataContext;

            //this.ClearSelection();
            this.SelectedFlags = SelectedFlags.Cell | SelectedFlags.Multiple;
            this.selectedItemRange.EndIndex = gridContext.Items.IndexOf(cell.DataContext);
            this.selectedColumnRange.EndIndex = gridContext.Columns[cell.FieldName].VisiblePosition;
            this.selectedGridContext = gridContext;
            cell.Focus();
            this.selectedGridContext.SelectedCellRanges.RemoveAt(this.selectedGridContext.SelectedCellRanges.Count - 1);
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange, this.selectedColumnRange));
            this.selectedGridContext.CurrentItem = row;
            this.selectedGridContext.CurrentColumn = column;
            this.NavigationBehavior = NavigationBehavior.CellOnly;
        }

        internal void Select(ModernDataCell cell)
        {
            var gridContext = DataGridControl.GetDataGridContext(cell);
            var column = gridContext.Columns[cell.FieldName];
            var row = cell.DataContext;

            this.ClearSelection();
            this.SelectedFlags = SelectedFlags.Cell;
            this.selectedItemRange = new SelectionRange(gridContext.Items.IndexOf(row));
            this.selectedColumnRange = new SelectionRange(column.VisiblePosition);
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange.StartIndex, this.selectedColumnRange.StartIndex));
        }

        internal void AddSelection(ModernDataCell cell)
        {
            var gridContext = ModernDataGridControl.GetDataGridContext(cell);
            if (this.selectedGridContext != gridContext)
                return;

            //this.NavigationBehavior = NavigationBehavior.None;
            var column = gridContext.Columns[cell.FieldName];
            var row = cell.DataContext;
            this.SelectedFlags |= SelectedFlags.Cell;
            this.selectedItemRange = new SelectionRange(gridContext.Items.IndexOf(row));
            this.selectedColumnRange = new SelectionRange(column.VisiblePosition);
            this.selectedItemRange.EndIndex = gridContext.Items.IndexOf(cell.DataContext);
            this.selectedColumnRange.EndIndex = gridContext.Columns[cell.FieldName].VisiblePosition;
            this.selectedGridContext = gridContext;
            this.selectedGridContext.SelectedCellRanges.Add(new SelectionCellRange(this.selectedItemRange.StartIndex, this.selectedColumnRange.StartIndex));
            this.selectedItemRange.StartIndex = this.selectedItemRange.EndIndex;
            this.selectedColumnRange.StartIndex = this.selectedColumnRange.EndIndex;
            //this.selectedGridContext.CurrentItem = row;
            //this.selectedGridContext.CurrentColumn = column;
            //this.NavigationBehavior = NavigationBehavior.CellOnly;
            //cell.Focus();
        }

        internal void ClearSelection()
        {
            if (this.selectedGridContext != null)
            {
                this.SelectedFlags = SelectedFlags.None;
                this.selectedGridContext.SelectedCellRanges.Clear();
                this.selectedGridContext.SelectedItemRanges.Clear();
            }
            this.selectedGridContext = null;
        }

        internal bool InvokeValueChangingEvent(ModernDataCell cell, object content)
        {
            var e = new ValueChangingEventArgs(cell.ParentRow.DataContext, cell.ParentColumn.FieldName, content)
            {
                RoutedEvent = ModernDataGridControl.ValueChangingEvent,
                Source = this,
            };
            this.RaiseEvent(e);

            return e.Cancel == false;
        }

        internal bool InvokeItemDeletingEvent(object item)
        {
            var e = new ItemDeletingEventArgs(item)
            {
                RoutedEvent = ModernDataGridControl.ItemDeletingEvent,
                Source = this,
            };

            this.OnPreviewItemDeleting(e);

            if (e.Handled == false)
            {
                this.RaiseEvent(e);
            }

            return e.Cancel == false;
        }
    }
}
