using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.DataGrid;

namespace Ntreev.ModernUI.Framework.DataGrid.Controls
{
    [TemplatePart(Name = "PART_InsertButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_InsertManyButton", Type = typeof(Button))]
    public class ModernInsertionRow : Control
    {
        private Button insertionButton;
        private Button insertionManyButton;

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

            this.insertionButton = this.Template.FindName("PART_InsertButton", this) as Button;
            this.insertionManyButton = this.Template.FindName("PART_InsertManyButton", this) as Button;
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

