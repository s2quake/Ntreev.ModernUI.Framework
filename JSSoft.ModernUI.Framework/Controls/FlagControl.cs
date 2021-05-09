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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework.Controls
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(FlagControlItem))]
    [TemplatePart(Name = "PART_EditableTextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class FlagControl : ItemsControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(long?), typeof(FlagControl),
                new UIPropertyMetadata(null, ValuePropertyChangedCallback, ValuePropertyCoerceValueCallback));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(FlagControl),
                new UIPropertyMetadata(string.Empty, TextPropertyChangedCallback));

        public static readonly DependencyProperty IsEditableProperty =
            DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(FlagControl));

        private static readonly DependencyPropertyKey SelectedItemsPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(SelectedItems), typeof(IList), typeof(FlagControl), new PropertyMetadata());
        public static readonly DependencyProperty SelectedItemsProperty = SelectedItemsPropertyKey.DependencyProperty;

        public static readonly DependencyProperty NumericMemberPathProperty =
            DependencyProperty.Register(nameof(NumericMemberPath), typeof(string), typeof(FlagControl),
                new UIPropertyMetadata(string.Empty, NumericMemberPathPropertyChangedCallback, NumericMemberPathPropertyCoerceValueCallback));

        public static readonly DependencyProperty CommentMemberPathProperty =
            DependencyProperty.Register(nameof(CommentMemberPath), typeof(string), typeof(FlagControl),
                new UIPropertyMetadata(string.Empty, CommentMemberPathPropertyChangedCallback, CommentMemberPathPropertyCoerceValueCallback));

        public static readonly DependencyProperty MaxDropDownHeightProperty =
            DependencyProperty.Register(nameof(MaxDropDownHeight), typeof(double), typeof(FlagControl));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(FlagControl),
                new UIPropertyMetadata(false, IsDropDownOpenPropertyChangedCallback));

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(nameof(SelectionChanged), RoutingStrategy.Bubble, typeof(SelectionChangedEventHandler), typeof(FlagControl));

        private readonly ObservableCollection<object> selectedItems = new();
        private Popup popup;

        private bool isContainerUpdating;
        private bool isValueUpdating;
        private bool isTextUpdating;
        private bool isSelectedItemsUpdating;

        static FlagControl()
        {
            EventManager.RegisterClassHandler(typeof(FlagControl), Mouse.MouseDownEvent, new MouseButtonEventHandler(Mouse_MouseDownHandler), true);
        }

        public FlagControl()
        {
            this.SetValue(MaxDropDownHeightProperty, 360.0);
            this.SetValue(SelectedItemsPropertyKey, this.selectedItems);

            this.selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
        }

        public long? Value
        {
            get => (long?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsEditable
        {
            get => (bool)this.GetValue(IsEditableProperty);
            set => this.SetValue(IsEditableProperty, value);
        }

        public string NumericMemberPath
        {
            get => (string)GetValue(NumericMemberPathProperty);
            set => SetValue(NumericMemberPathProperty, value);
        }

        public string CommentMemberPath
        {
            get => (string)this.GetValue(CommentMemberPathProperty);
            set => this.SetValue(CommentMemberPathProperty, value);
        }

        public IList SelectedItems => (IList)GetValue(SelectedItemsProperty);

        public double MaxDropDownHeight
        {
            get => (double)GetValue(MaxDropDownHeightProperty);
            set => SetValue(MaxDropDownHeightProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)GetValue(IsDropDownOpenProperty);
            set => SetValue(IsDropDownOpenProperty, value);
        }

        public event SelectionChangedEventHandler SelectionChanged
        {
            add { AddHandler(SelectionChangedEvent, value); }
            remove { RemoveHandler(SelectionChangedEvent, value); }
        }

        public event EventHandler DropDownClosed;

        public event EventHandler DropDownOpened;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.popup != null)
            {
                this.popup.Closed -= Popup_Closed;
            }

            this.popup = this.Template.FindName("PART_Popup", this) as Popup;

            if (this.popup != null)
            {
                this.popup.Closed += Popup_Closed;
            }

            this.UpdateSelectedItems();
            this.UpdateText();
        }

        internal void AddFlag(long value)
        {
            if (this.isValueUpdating == true)
                return;

            this.isValueUpdating = true;

            try
            {
                if (this.Value.HasValue == false)
                    this.Value = value;
                else if (value == 0)
                    this.Value = 0;
                else
                    this.Value |= value;
            }
            finally
            {
                this.isValueUpdating = false;
            }
        }

        internal void RemoveFlag(long value)
        {
            if (this.isValueUpdating == true)
                return;

            this.isValueUpdating = true;

            try
            {
                if (this.Value.HasValue == false)
                    return;

                if (this.Value == value)
                    this.Value = null;
                else
                    this.Value = (long)this.Value & ~value;
            }
            finally
            {
                this.isValueUpdating = false;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new FlagControlItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is FlagControlItem;
        }

        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);

            if (element != item)
            {
                BindingOperations.SetBinding(element, FlagControlItem.ValueProperty, new Binding(this.NumericMemberPath));
                if (this.CommentMemberPath != string.Empty)
                    BindingOperations.SetBinding(element, FlagControlItem.ToolTipProperty, new Binding(this.CommentMemberPath));
            }
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            this.UpdateSelectedItems();
            this.UpdateText();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Escape)
            {
                if (this.IsDropDownOpen == true)
                    this.IsDropDownOpen = false;
            }
            else if (e.Key == Key.F2)
            {
                if (this.IsDropDownOpen == false)
                    this.IsDropDownOpen = true;
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            if (this.popup == null)
                return;

            if (Mouse.Captured == this && e.OriginalSource == this && this.popup.IsMouseOver == false)
            {
                if (this.IsDropDownOpen == true)
                    this.IsDropDownOpen = false;
            }
        }

        protected override void OnIsKeyboardFocusWithinChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsKeyboardFocusWithinChanged(e);

            if (this.IsDropDownOpen == true)
                this.IsDropDownOpen = false;
        }

        protected override void OnIsMouseCapturedChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnIsMouseCapturedChanged(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected virtual void OnDropDownClosed(EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            this.DropDownOpened?.Invoke(this, e);
        }

        protected long GetNumericMember(object item)
        {
            if (item is FlagControlItem == true)
            {
                return (item as FlagControlItem).Value;
            }

            var value = PropertyPathHelper.GetValue(item, this.NumericMemberPath);
            return Convert.ToInt64(value);
        }

        private string GetDisplayMember(object item)
        {
            string text;
            if (item is ContentControl)
            {
                text = (item as ContentControl).Content.ToString();
            }
            else
            {
                text = PropertyPathHelper.GetValue(item, this.DisplayMemberPath).ToString();
            }

            if (text.IndexOf(' ') >= 0)
                return "\"" + text + "\"";

            return text;
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlagControl;
            control.UpdateContainers();
            control.UpdateText();
            control.UpdateSelectedItems();
        }

        private static object ValuePropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return baseValue;

            var control = d as FlagControl;
            var controlValue = (long)baseValue;

            if (controlValue == 0)
                return baseValue;

            foreach (var item in control.Items)
            {
                var itemValue = control.GetNumericMember(item);
                if (itemValue == 0L)
                    continue;
                if ((controlValue & itemValue) == itemValue)
                    return baseValue;
            }
            return 0L;
        }

        private static object NumericMemberPathPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return string.Empty;
            return baseValue;
        }

        private static void NumericMemberPathPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static object CommentMemberPathPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null)
                return string.Empty;
            return baseValue;
        }

        private static void CommentMemberPathPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

        }

        private static void TextPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FlagControl;
            control.UpdateValue((string)e.NewValue);
        }

        private static void IsDropDownOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flagControl = d as FlagControl;
            if ((bool)e.NewValue == true)
            {
                Mouse.Capture(flagControl, CaptureMode.SubTree);
                flagControl.OnDropDownOpened(EventArgs.Empty);
            }
            else
            {
                if (Mouse.Captured == flagControl)
                {
                    Mouse.Capture(null);
                }
            }
        }

        private static void Mouse_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            var flagControl = sender as FlagControl;

            if (flagControl.IsDropDownOpen == true && (flagControl.popup.IsMouseOver == false && flagControl.IsMouseOver == false))
            {
                flagControl.IsDropDownOpen = false;
            }
        }

        private void UpdateContainers()
        {
            if (this.isContainerUpdating == true)
                return;

            this.isContainerUpdating = true;

            try
            {
                for (var i = 0; i < this.ItemContainerGenerator.Items.Count; i++)
                {
                    if (!(this.ItemContainerGenerator.ContainerFromIndex(i) is FlagControlItem item))
                        continue;

                    if (this.Value.HasValue == true)
                    {
                        var value = (long)this.Value;
                        var itemValue = item.Value;
                        if (itemValue == 0)
                        {
                            item.IsSelected = value == 0;
                        }
                        else
                        {
                            item.IsSelected = (value & itemValue) == itemValue;
                        }
                    }
                    else
                    {
                        item.IsSelected = false;
                    }
                }
            }
            finally
            {
                this.isContainerUpdating = false;
            }
        }

        private void UpdateSelectedItems()
        {
            if (this.isSelectedItemsUpdating == true)
                return;

            this.isSelectedItemsUpdating = true;
            this.selectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            try
            {
                var addedItems = new List<object>();
                var removedItems = new List<object>(this.selectedItems);

                this.selectedItems.Clear();
                if (this.Value == 0)
                {
                    foreach (var item in this.Items)
                    {
                        var itemValue = this.GetNumericMember(item);
                        if (itemValue == 0)
                        {
                            this.selectedItems.Add(item);
                            break;
                        }
                    }
                }
                else
                {
                    var query = from object item in this.Items
                                let itemValue = this.GetNumericMember(item)
                                where itemValue != (long)0 && (this.Value & itemValue) == itemValue
                                select item;

                    foreach (var item in query)
                    {
                        this.selectedItems.Add(item);
                    }
                }

                foreach (var item in this.selectedItems)
                {
                    removedItems.Remove(item);
                }

                foreach (var item in this.selectedItems)
                {
                    if (removedItems.Contains(item) == false)
                        addedItems.Add(item);
                }

                this.RaiseEvent(new SelectionChangedEventArgs(SelectionChangedEvent, removedItems, addedItems));
            }
            finally
            {
                this.selectedItems.CollectionChanged += SelectedItems_CollectionChanged;
                this.isSelectedItemsUpdating = false;
            }
        }

        private void UpdateText()
        {
            if (this.isTextUpdating == true)
                return;

            this.isTextUpdating = true;
            try
            {
                var texts = new List<object>();
                if (this.Value.HasValue == true)
                {
                    if (this.Value == 0)
                    {
                        foreach (var item in this.Items)
                        {
                            var itemValue = (long)this.GetNumericMember(item);
                            if (itemValue == 0)
                            {
                                texts.Add(this.GetDisplayMember(item));
                                break;
                            }
                        }
                    }
                    else
                    {
                        var query = from object item in this.Items
                                    let itemValue = this.GetNumericMember(item)
                                    where itemValue != 0
                                    orderby (ulong)itemValue descending
                                    select new KeyValuePair<object, long>(item, itemValue);

                        long value = (long)this.Value;
                        foreach (var item in query)
                        {
                            object obj = item.Key;
                            long itemValue = item.Value;
                            if ((value & item.Value) == item.Value)
                            {
                                texts.Add(this.GetDisplayMember(obj));
                                value &= ~item.Value;
                            }
                        }
                    }
                }
                this.Text = string.Join(" ", texts);
            }
            finally
            {
                this.isTextUpdating = false;
            }
        }

        private void SelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {

        }

        private void UpdateValue(string newText)
        {
            if (this.isTextUpdating == true)
                return;

            this.isTextUpdating = true;

            const string pattern = "(\".*?\")|(\\S*)";

            try
            {
                if (newText != null)
                {
                    var matches = Regex.Matches(newText, pattern);

                    Dictionary<string, long> values = new(this.Items.Count);

                    foreach (var item in this.Items)
                    {
                        values.Add(this.GetDisplayMember(item), this.GetNumericMember(item));
                    }

                    foreach (Match item in matches)
                    {
                        if (item.Value != string.Empty)
                        {
                            if (values.ContainsKey(item.Value) == false)
                            {
                                this.Value = null;
                                return;
                            }
                        }
                    }

                    long value = 0;
                    foreach (Match item in matches)
                    {
                        if (item.Value != string.Empty)
                        {
                            value |= values[item.Value];
                        }
                    }

                    this.Value = value;
                }
                else
                {
                    this.Value = null;
                }
            }
            finally
            {
                this.isTextUpdating = false;
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            this.OnDropDownClosed(e);
        }
    }
}
