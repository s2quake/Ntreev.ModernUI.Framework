using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = "PART_Left", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Top", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Right", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_Bottom", Type = typeof(TextBox))]
    public class ThicknessControl : UserControl
    {
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(Thickness), typeof(ThicknessControl),
                new FrameworkPropertyMetadata(ThicknessPropertyChangedCallback));

        private TextBox leftControl;
        private TextBox topControl;
        private TextBox rightControl;
        private TextBox bottomControl;

        private bool isUpdating;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.leftControl = this.Template.FindName("PART_Left", this) as TextBox;
            if (this.leftControl != null)
            {
                this.leftControl.Text = $"{this.Thickness.Left}";
                this.AttachEvent(this.leftControl);
            }
            this.topControl = this.Template.FindName("PART_Top", this) as TextBox;
            if (this.topControl != null)
            {
                this.topControl.Text = $"{this.Thickness.Top}";
                this.AttachEvent(this.topControl);
            }
            this.rightControl = this.Template.FindName("PART_Right", this) as TextBox;
            if (this.rightControl != null)
            {
                this.rightControl.Text = $"{this.Thickness.Right}";
                this.AttachEvent(this.rightControl);
            }
            this.bottomControl = this.Template.FindName("PART_Bottom", this) as TextBox;
            if (this.bottomControl != null)
            {
                this.bottomControl.Text = $"{this.Thickness.Bottom}";
                this.AttachEvent(this.bottomControl);
            }
        }

        public Thickness Thickness
        {
            get => (Thickness)this.GetValue(ThicknessProperty);
            set => this.SetValue(ThicknessProperty, value);
        }

        private static void ThicknessPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ThicknessControl self)
            {
                self.UpdateValue();
            }
        }

        private void UpdateValue()
        {
            if (this.isUpdating == true)
                return;

            if (this.leftControl != null)
            {
                this.leftControl.Text = $"{this.Thickness.Left}";
            }
            if (this.topControl != null)
            {
                this.topControl.Text = $"{this.Thickness.Top}";
            }
            if (this.rightControl != null)
            {
                this.rightControl.Text = $"{this.Thickness.Right}";
            }
            if (this.bottomControl != null)
            {
                this.bottomControl.Text = $"{this.Thickness.Bottom}";
            }
        }

        private void AttachEvent(TextBox textBox)
        {
            textBox.PreviewTextInput += TextBox_PreviewTextInput;
            textBox.KeyDown += TextBox_KeyDown;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.GotFocus += TextBox_GotFocus;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var left = this.Thickness.Left;
                var top = this.Thickness.Top;
                var right = this.Thickness.Right;
                var bottom = this.Thickness.Bottom;
                if (this.leftControl == textBox)
                {
                    left = double.Parse(this.leftControl.Text);
                }
                else if (this.topControl == textBox)
                {
                    top = double.Parse(this.topControl.Text);
                }
                else if (this.rightControl == textBox)
                {
                    right = double.Parse(this.rightControl.Text);
                }
                else if (this.bottomControl == textBox)
                {
                    bottom = double.Parse(this.bottomControl.Text);
                }
                this.isUpdating = true;
                this.Thickness = new Thickness(left, top, right, bottom);
                this.isUpdating = false;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                var text = textBox.Text.Insert(textBox.CaretIndex, e.Text);
                if (double.TryParse(text, out _) == false)
                {
                    e.Handled = true;
                }
            }
        }
    }
}
