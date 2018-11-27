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
    [TemplatePart(Name = PART_EditableTextBox, Type = typeof(TextBox))]
    public class GuidControl : Control
    {
        public const string PART_EditableTextBox = "PART_EditableTextBox";

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(Guid?), typeof(GuidControl),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValuePropertychangedCallback));

        public static readonly RoutedCommand NewCommand = new RoutedUICommand("New", nameof(NewCommand), typeof(GuidControl));

        private TextBox textBox;

        public GuidControl()
        {
            this.CommandBindings.Add(new CommandBinding(NewCommand, NewCommand_Executed));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.textBox = (TextBox)this.Template.FindName(PART_EditableTextBox, this);
            if (this.textBox != null)
            {
                this.textBox.Text = $"{this.Value}";
                this.textBox.PreviewTextInput += TextBox_PreviewTextInput;
                this.textBox.PreviewKeyDown += TextBox_PreviewKeyDown;

                CommandManager.AddPreviewExecutedHandler(this.textBox, ExecutedRoutedEventHandler);
            }
        }

        public Guid? Value
        {
            get { return (Guid?)this.GetValue(ValueProperty); }
            set { this.SetValue(ValueProperty, value); }
        }

        private void NewCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Value = Guid.NewGuid();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = true;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
        }

        private void ExecutedRoutedEventHandler(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Paste)
            {
                var text = Clipboard.GetText();
                if (Guid.TryParse(text, out var guid) == false)
                {
                    e.Handled = true;
                    this.Value = guid;
                }
            }
        }

        private static void ValuePropertychangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GuidControl control)
            {
                if (e.NewValue is Guid guid)
                {
                    if (control.textBox != null)
                        control.textBox.Text = $"{guid}";
                }
                else
                {
                    if (control.textBox != null)
                        control.textBox.Text = string.Empty;
                }
            }
        }
    }
}
