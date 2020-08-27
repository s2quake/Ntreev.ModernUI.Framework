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

using System.Windows;
using System.Windows.Controls;

namespace JSSoft.ModernUI.Framework.Controls
{
    public class FlagControlItem : ContentControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(long), typeof(FlagControlItem),
                new FrameworkPropertyMetadata(ValuePropertyChangedCallback));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(FlagControlItem),
                new FrameworkPropertyMetadata(true, IsSelectedPropertyChangedCallback));

        public static readonly DependencyProperty FlagTypeProperty =
            DependencyProperty.Register(nameof(FlagType), typeof(FlagControlItemType), typeof(FlagControlItem));

        public static RoutedEvent SelectedEvent = EventManager.RegisterRoutedEvent("Selected", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(FlagControlItem));

        public static RoutedEvent UnselectedEvent = EventManager.RegisterRoutedEvent("Unselected", RoutingStrategy.Bubble,
            typeof(RoutedEventHandler), typeof(FlagControlItem));

        private bool isUpdating;

        public FlagControlItem()
        {

        }

        public long Value
        {
            get => (long)this.GetValue(ValueProperty);
            set => this.SetValue(ValueProperty, value);
        }

        public bool IsSelected
        {
            get => (bool)this.GetValue(IsSelectedProperty);
            set => this.SetValue(IsSelectedProperty, value);
        }

        public FlagControlItemType FlagType => (FlagControlItemType)this.GetValue(FlagTypeProperty);

        public event RoutedEventHandler Selected
        {
            add { AddHandler(SelectedEvent, value); }
            remove { RemoveHandler(SelectedEvent, value); }
        }

        public event RoutedEventHandler Unselected
        {
            add { AddHandler(UnselectedEvent, value); }
            remove { RemoveHandler(UnselectedEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ItemsControl.ItemsControlFromItemContainer(this) is FlagControl flagControl)
            {
                this.isUpdating = true;
                try
                {
                    if (flagControl.Value.HasValue == true)
                    {
                        long controlValue = (long)flagControl.Value;
                        if (this.Value == 0)
                            this.IsSelected = controlValue == this.Value;
                        else
                            this.IsSelected = (controlValue & this.Value) == this.Value;
                    }
                    else
                    {
                        this.IsSelected = false;
                    }
                }
                finally
                {
                    this.isUpdating = false;
                }
            }
        }

        private static void ValuePropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            _ = ItemsControl.ItemsControlFromItemContainer(d) as FlagControl;
            var value = (long)e.NewValue;

            if (value == 0)
            {
                d.SetValue(FlagTypeProperty, FlagControlItemType.None);
            }
            else if (value == -1)
            {
                d.SetValue(FlagTypeProperty, FlagControlItemType.All);
            }
            else
            {
                var b = (value & (value - 1)) == 0;
                if (b == true)
                    d.SetValue(FlagTypeProperty, FlagControlItemType.Single);
                else
                    d.SetValue(FlagTypeProperty, FlagControlItemType.Multiple);
            }
        }

        private static void IsSelectedPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var flagItem = d as FlagControlItem;
            var isSelected = (bool)e.NewValue;

            if (ItemsControl.ItemsControlFromItemContainer(d) is FlagControl flagControl && flagItem.isUpdating == false)
            {
                if (isSelected == true)
                    flagControl.AddFlag(flagItem.Value);
                else
                    flagControl.RemoveFlag(flagItem.Value);
            }

            if (isSelected == true)
            {
                flagItem.RaiseEvent(new RoutedEventArgs(SelectedEvent));
            }
            else
            {
                flagItem.RaiseEvent(new RoutedEventArgs(UnselectedEvent));
            }
        }
    }
}
