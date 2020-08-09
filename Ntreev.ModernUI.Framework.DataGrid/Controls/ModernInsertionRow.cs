using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    [TemplatePart(Name = PART_InsertButton, Type = typeof(Button))]
    [TemplatePart(Name = PART_InsertManyButton, Type = typeof(Button))]
    public class ModernInsertionRow : Control
    {
        public const string PART_InsertButton = nameof(PART_InsertButton);
        public const string PART_InsertManyButton = nameof(PART_InsertManyButton);

        public static readonly DependencyProperty InsertCommandProperty =
            DependencyProperty.Register(nameof(InsertCommand), typeof(ICommand), typeof(ModernInsertionRow));

        public static readonly DependencyProperty InsertManyCommandProperty =
            DependencyProperty.Register(nameof(InsertManyCommand), typeof(ICommand), typeof(ModernInsertionRow));

        public static readonly DependencyProperty ColumnManagerRowProperty =
            DependencyProperty.Register(nameof(ColumnManagerRow), typeof(ModernColumnManagerRow), typeof(ModernInsertionRow),
                new UIPropertyMetadata(ColumnManagerRowPropertyChangedCallback));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        public ICommand InsertCommand
        {
            get => (ICommand)this.GetValue(InsertCommandProperty);
            set => this.SetValue(InsertCommandProperty, value);
        }

        public ICommand InsertManyCommand
        {
            get => (ICommand)this.GetValue(InsertManyCommandProperty);
            set => this.SetValue(InsertManyCommandProperty, value);
        }

        public ModernColumnManagerRow ColumnManagerRow
        {
            get => (ModernColumnManagerRow)this.GetValue(ColumnManagerRowProperty);
            set => this.SetValue(ColumnManagerRowProperty, value);
        }

        private static void ColumnManagerRowPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
    }
}
