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

using JSSoft.ModernUI.Framework.Properties;
using System.Windows.Input;

namespace JSSoft.ModernUI.Framework.DataGrid.Controls
{
    public static class ModernDataGridCommands
    {
        public static readonly RoutedCommand InsertItem = new RoutedCommand(nameof(InsertItem), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand CopyWithHeaders = new RoutedCommand(nameof(CopyWithHeaders), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand SelectInsertion = new RoutedCommand(nameof(SelectInsertion), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand MoveToNextItem = new RoutedCommand(nameof(MoveToNextItem), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand MoveToPrevItem = new RoutedCommand(nameof(MoveToPrevItem), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand MoveToNextColumn = new RoutedCommand(nameof(MoveToNextColumn), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand MoveToPrevColumn = new RoutedCommand(nameof(MoveToPrevColumn), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand NextMatchedItem = new RoutedUICommand(Resources.Command_NextSearchedItem, nameof(NextMatchedItem), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand PrevMatchedItem = new RoutedUICommand(Resources.Command_PrevSearchedItem, nameof(PrevMatchedItem), typeof(ModernDataGridCommands));

        public static readonly RoutedCommand NewLine = new RoutedCommand(nameof(NewLine), typeof(ModernDataGridCommands));

        static ModernDataGridCommands()
        {
            InsertItem.InputGestures.Add(new KeyGesture(Key.Enter));
            CopyWithHeaders.InputGestures.Add(new KeyGesture(Key.C, ModifierKeys.Control | ModifierKeys.Shift));
            SelectInsertion.InputGestures.Add(new KeyGesture(Key.F12));

            MoveToNextItem.InputGestures.Add(new KeyGesture(Key.Return));
            MoveToPrevItem.InputGestures.Add(new KeyGesture(Key.Return, ModifierKeys.Shift));
            MoveToNextColumn.InputGestures.Add(new KeyGesture(Key.Tab));
            MoveToPrevColumn.InputGestures.Add(new KeyGesture(Key.Tab, ModifierKeys.Shift));
            NewLine.InputGestures.Add(new KeyGesture(Key.Enter, ModifierKeys.Alt));
        }
    }
}
