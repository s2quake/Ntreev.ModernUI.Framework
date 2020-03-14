using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_Left, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Top, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Right, Type = typeof(TextBox))]
    [TemplatePart(Name = PART_Bottom, Type = typeof(TextBox))]
    public class ThicknessControl : UserControl
    {
        public const string PART_Left = nameof(PART_Left);
        public const string PART_Top = nameof(PART_Top);
        public const string PART_Right = nameof(PART_Right);
        public const string PART_Bottom = nameof(PART_Bottom);

        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Value), typeof(Thickness), typeof(ThicknessControl),
                new FrameworkPropertyMetadata(ThicknessPropertyChangedCallback));

        private TextBox leftControl;
        private TextBox topControl;
        private TextBox rightControl;
        private TextBox bottomControl;

        private bool isUpdating;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.leftControl = this.Template.FindName(PART_Left, this) as TextBox;
            if (this.leftControl != null)
            {
                this.leftControl.Text = $"{this.Value.Left}";
                this.AttachEvent(this.leftControl);
            }
            this.topControl = this.Template.FindName(PART_Top, this) as TextBox;
            if (this.topControl != null)
            {
                this.topControl.Text = $"{this.Value.Top}";
                this.AttachEvent(this.topControl);
            }
            this.rightControl = this.Template.FindName(PART_Right, this) as TextBox;
            if (this.rightControl != null)
            {
                this.rightControl.Text = $"{this.Value.Right}";
                this.AttachEvent(this.rightControl);
            }
            this.bottomControl = this.Template.FindName(PART_Bottom, this) as TextBox;
            if (this.bottomControl != null)
            {
                this.bottomControl.Text = $"{this.Value.Bottom}";
                this.AttachEvent(this.bottomControl);
            }
        }

        public Thickness Value
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
                this.leftControl.Text = $"{this.Value.Left}";
            }
            if (this.topControl != null)
            {
                this.topControl.Text = $"{this.Value.Top}";
            }
            if (this.rightControl != null)
            {
                this.rightControl.Text = $"{this.Value.Right}";
            }
            if (this.bottomControl != null)
            {
                this.bottomControl.Text = $"{this.Value.Bottom}";
            }
        }

        private void AttachEvent(TextBox textBox)
        {
            //textBox.PreviewTextInput += TextBox_PreviewTextInput;
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
                var left = this.Value.Left;
                var top = this.Value.Top;
                var right = this.Value.Right;
                var bottom = this.Value.Bottom;
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
                this.Value = new Thickness(left, top, right, bottom);
                this.isUpdating = false;
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
