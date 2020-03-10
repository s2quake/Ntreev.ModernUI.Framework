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
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    public class IconButton : Button
    {
        public const string PART_Popup = nameof(PART_Popup);

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(nameof(Source), typeof(ImageSource), typeof(IconButton));

        public static readonly DependencyProperty DropDownTemplateProperty =
            DependencyProperty.Register(nameof(DropDownTemplate), typeof(DataTemplate), typeof(IconButton));

        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(IconButton),
                new FrameworkPropertyMetadata(false, IsDropDownOpenPropertyChangedCallback));

        private Popup popup;

        static IconButton()
        {
            EventManager.RegisterClassHandler(typeof(IconButton), Mouse.MouseDownEvent, new MouseButtonEventHandler(Mouse_MouseDownHandler), true);
        }

        public IconButton()
        {

        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.popup != null)
            {
                this.popup.Closed -= Popup_Closed;
                this.popup.Opened -= Popup_Opened;
            }
            this.popup = this.Template.FindName(PART_Popup, this) as Popup;
            if (this.popup != null)
            {
                this.popup.Closed += Popup_Closed;
                this.popup.Opened += Popup_Opened;
            }
        }

        public ImageSource Source
        {
            get => (ImageSource)this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

        public DataTemplate DropDownTemplate
        {
            get => (DataTemplate)this.GetValue(DropDownTemplateProperty);
            set => this.SetValue(DropDownTemplateProperty, value);
        }

        public bool IsDropDownOpen
        {
            get => (bool)this.GetValue(IsDropDownOpenProperty);
            set => this.SetValue(IsDropDownOpenProperty, value);
        }

        public event EventHandler DropDownClosed;

        public event EventHandler DropDownOpened;

        protected virtual void OnDropDownClosed(EventArgs e)
        {
            this.DropDownClosed?.Invoke(this, e);
        }

        protected virtual void OnDropDownOpened(EventArgs e)
        {
            this.DropDownOpened?.Invoke(this, e);
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (this.IsDropDownOpen == true)
                this.IsDropDownOpen = false;
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

            //if (this.IsDropDownOpen == true)
            //    this.IsDropDownOpen = false;
        }

        private static void IsDropDownOpenPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is IconButton self)
            {
                if ((bool)e.NewValue == true)
                {
                    Mouse.Capture(self, CaptureMode.SubTree);
                    self.OnDropDownOpened(EventArgs.Empty);
                }
                else
                {
                    if (Mouse.Captured == self)
                    {
                        Mouse.Capture(null);
                    }
                }
            }
        }

        private static void Mouse_MouseDownHandler(object sender, MouseButtonEventArgs e)
        {
            if (sender is IconButton button)
            {
                if (button.IsDropDownOpen == true && (button.popup.IsMouseOver == false && button.IsMouseOver == false))
                {
                    button.IsDropDownOpen = false;
                }
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            this.OnDropDownClosed(e);
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            this.popup.IsKeyboardFocusWithinChanged += Popup_IsKeyboardFocusWithinChanged;
        }

        private void Popup_IsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                this.IsDropDownOpen = false;
            }
        }
    }
}
