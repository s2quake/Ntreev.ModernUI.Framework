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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = PART_TextBox, Type = typeof(RichTextBox))]
    public class TerminalControl : Control
    {
        public const string PART_TextBox = nameof(PART_TextBox);

        public readonly static DependencyProperty PromptProperty =
            DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(TerminalControl),
                new FrameworkPropertyMetadata(string.Empty, PromptPropertyChangedCallback, PromptPropertyCoerceValueCallback));

        private readonly static DependencyPropertyKey TextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Text), typeof(string), typeof(TerminalControl),
                new FrameworkPropertyMetadata(string.Empty));
        public readonly static DependencyProperty TextProperty = TextPropertyKey.DependencyProperty;

        public readonly static DependencyProperty OutputForegroundProperty =
            DependencyProperty.Register(nameof(OutputForeground), typeof(Brush), typeof(TerminalControl),
                new FrameworkPropertyMetadata(OutputForegroundPropertyChangedCallback));

        public readonly static DependencyProperty OutputBackgroundProperty =
            DependencyProperty.Register(nameof(OutputBackground), typeof(Brush), typeof(TerminalControl),
                new FrameworkPropertyMetadata(OutputBackgroundPropertyChangedCallback));

        private RichTextBox textBox;

        private readonly List<string> histories = new List<string>();
        private readonly List<string> completions = new List<string>();
        private int historyIndex;

        private Paragraph promptBlock;
        private Paragraph outputBlock;
        private Run output;
        private string inputText = string.Empty;
        private string outputLeftText = string.Empty;
        private int refStack = 0;
        private string completion;

        private bool isChanged;

        public readonly static RoutedEvent ExecutedEvent =
            EventManager.RegisterRoutedEvent(nameof(Executed), RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(TerminalControl));

        public TerminalControl()
        {
        }

        public event RoutedEventHandler Executed
        {
            add { AddHandler(ExecutedEvent, value); }
            remove { RemoveHandler(ExecutedEvent, value); }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.textBox = this.Template.FindName("PART_TextBox", this) as RichTextBox;

            if (this.textBox != null)
            {
                this.textBox.Document.Blocks.Clear();
                this.promptBlock = new Paragraph();
                this.textBox.Document.Blocks.Add(this.promptBlock);
                this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                this.textBox.CaretPosition = this.promptBlock.ContentEnd;
                this.textBox.TextChanged += TextBox_TextChanged;
                this.textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
                this.textBox.SelectionChanged += TextBox_SelectionChanged;
            }
        }

        public void Execute()
        {
            this.refStack++;
            try
            {
                var commandText = this.Command;
                var args = new RoutedEventArgs(ExecutedEvent);
                this.SetValue(TextPropertyKey, commandText);
                if (this.histories.Contains(this.Text) == false)
                {
                    this.histories.Add(this.Text);
                    this.historyIndex = this.histories.Count;
                }
                else
                {
                    this.historyIndex = this.histories.LastIndexOf(this.Text) + 1;
                }

                this.AppendLine(this.Prompt + commandText);

                this.promptBlock.Inlines.Clear();
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.textBox.IsReadOnly = true;

                this.OnExecuted(args);
                if (args.Handled == false)
                {
                    this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    this.textBox.CaretPosition = this.promptBlock.ContentEnd;
                    this.textBox.IsReadOnly = false;
                }
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Clear()
        {
            this.promptBlock.Inlines.Clear();
            this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
            this.promptBlock.Inlines.Add(new Run() { Text = string.Empty });
            this.inputText = string.Empty;
            this.completion = string.Empty;
        }

        public void MoveToFirst()
        {
            this.CursorPosition = 0;
        }

        public void MoveToLast()
        {
            this.CursorPosition = this.Command.Length;
        }

        public void MoveLeft()
        {
            if (this.CursorPosition > 0)
                this.CursorPosition--;
        }

        public void MoveRight()
        {
            if (this.CursorPosition < this.Command.Length)
                this.CursorPosition++;
        }

        public void Reset()
        {
            this.refStack++;
            try
            {
                this.output = null;
                this.outputBlock = null;
                this.outputLeftText = string.Empty;
                this.textBox.Document.Blocks.Clear();
                this.promptBlock = new Paragraph();
                this.textBox.Document.Blocks.Add(this.promptBlock);
                this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                this.textBox.CaretPosition = this.promptBlock.Inlines.LastInline.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Append(string text)
        {
            this.AppendInternal(text);
        }

        public void AppendLine(string text)
        {
            this.AppendInternal(text + Environment.NewLine);
        }

        public string Text => (string)this.GetValue(TextProperty);

        public string Prompt
        {
            get => (string)this.GetValue(PromptProperty);
            set => this.SetValue(PromptProperty, value);
        }

        public string Command
        {
            get
            {
                var prompt = this.Prompt;
                var pointer = this.promptBlock.ContentStart;
                var line = string.Empty;
                while (pointer != null)
                {
                    if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        var text = pointer.GetTextInRun(LogicalDirection.Forward);
                        line += text;
                    }
                    pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
                }
                return line.Substring(prompt.Length);
            }
            set
            {
                this.refStack++;
                try
                {
                    this.promptBlock.Inlines.Clear();
                    this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    this.promptBlock.Inlines.Add(new Run() { Text = value });
                    this.SetCursorPosition(0);
                }
                finally
                {
                    this.refStack--;
                }
            }
        }

        public int CursorPosition
        {
            get
            {
                var caretPosition = this.textBox.CaretPosition;
                if (caretPosition.CompareTo(this.promptBlock.ContentStart) < 0)
                {
                    return -1;
                }
                else
                {
                    var prompt = this.Prompt;
                    var position = caretPosition;
                    var line = string.Empty;
                    while (position != null && position.IsAtLineStartPosition == false)
                    {
                        if (position.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.Text)
                        {
                            var text = position.GetTextInRun(LogicalDirection.Backward);
                            var length = position.GetTextRunLength(LogicalDirection.Backward);
                            line = text.Substring(0, length) + line;
                        }
                        position = position.GetNextContextPosition(LogicalDirection.Backward);
                    }
                    if (line.Length < prompt.Length)
                        return -1;
                    return line.Length - prompt.Length;
                }
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();

                this.SetCursorPosition(value);
            }
        }

        public Brush OutputForeground
        {
            get => (Brush)this.GetValue(OutputForegroundProperty);
            set => this.SetValue(OutputForegroundProperty, value);
        }

        public Brush OutputBackground
        {
            get => (Brush)this.GetValue(OutputBackgroundProperty);
            set => this.SetValue(OutputBackgroundProperty, value);
        }

        public void NextCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(NextCompletion);
                this.textBox.CaretPosition = this.promptBlock.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void PrevCompletion()
        {
            this.refStack++;
            try
            {
                this.CompletionImpl(PrevCompletion);
                this.textBox.CaretPosition = this.promptBlock.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void NextHistory()
        {
            if (this.historyIndex + 1 < this.histories.Count)
            {
                this.inputText = this.Command = this.histories[this.historyIndex + 1];
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.Command = this.histories[this.historyIndex - 1];
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.Command = this.histories[0];
                this.MoveToLast();
                this.historyIndex = 0;
            }
        }

        public void InsertPrompt()
        {
            this.refStack++;
            try
            {
                var textRange = new TextRange(this.promptBlock.ContentStart, this.promptBlock.ContentEnd);
                var isEnd = this.textBox.CaretPosition.CompareTo(this.promptBlock.ContentEnd) == 0;
                if (textRange.Text != string.Empty)
                    this.AppendLine(textRange.Text);
                this.promptBlock.Inlines.Clear();
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                if (isEnd == true)
                    this.textBox.CaretPosition = this.promptBlock.ContentEnd;
                this.textBox.IsReadOnly = false;
            }
            finally
            {
                this.refStack--;
            }
        }

        public static Match[] MatchCompletion(string text)
        {
            var matches = Regex.Matches(text, "\\S+");
            var argList = new List<Match>();

            foreach (Match item in matches)
            {
                argList.Add(item);
            }

            return argList.ToArray();
        }

        public static string NextCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i + 1 < completions.Length)
                            return completions[i + 1];
                        else
                            return completions.First();
                    }
                }
            }
            else
            {
                for (var i = 0; i < completions.Length; i++)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        public static string PrevCompletion(string[] completions, string text)
        {
            completions = completions.OrderBy(item => item).ToArray();
            if (completions.Contains(text) == true)
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r == 0)
                    {
                        if (i - 1 >= 0)
                            return completions[i - 1];
                        else
                            return completions.Last();
                    }
                }
            }
            else
            {
                for (var i = completions.Length - 1; i >= 0; i--)
                {
                    var r = string.Compare(text, completions[i], true);
                    if (r < 0)
                    {
                        return completions[i];
                    }
                }
            }
            return text;
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
        }

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            this.textBox?.Focus();
        }

        protected virtual void OnExecuted(RoutedEventArgs e)
        {
            this.RaiseEvent(e);
        }

        protected virtual string[] GetCompletion(string[] items, string find)
        {
            var query = from item in this.completions
                        where item.StartsWith(find)
                        select item;
            return query.ToArray();
        }

        protected virtual Inline[] GetPrompt(string prompt)
        {
            return new Run[]
            {
                new Run(){ Text = this.Prompt, },
            };
        }

        private void CompletionImpl(Func<string[], string, string> func)
        {
            var matches = new List<Match>(CommandStringUtility.MatchCompletion(this.inputText));
            var find = string.Empty;
            var prefix = false;
            var postfix = false;
            var leftText = this.inputText;
            if (matches.Count > 0)
            {
                var match = matches.Last();
                var matchText = match.Value;
                if (matchText.Length > 0 && matchText.First() == '\"')
                {
                    prefix = true;
                    matchText = matchText.Substring(1);
                }
                if (matchText.Length > 1 && matchText.Last() == '\"')
                {
                    postfix = true;
                    matchText = matchText.Remove(matchText.Length - 1);
                }
                if (matchText == string.Empty || matchText.Trim() != string.Empty)
                {
                    find = matchText;
                    matches.RemoveAt(matches.Count - 1);
                    leftText = this.inputText.Remove(match.Index);
                }
            }

            var argList = new List<string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var matchText = CommandStringUtility.TrimQuot(matches[i].Value).Trim();
                if (matchText != string.Empty)
                    argList.Add(matchText);
            }

            var completions = this.GetCompletion(argList.ToArray(), find);
            if (completions != null && completions.Any())
            {
                this.completion = func(completions, this.completion);
                var inputText = this.inputText;

                if (prefix == true || postfix == true)
                {
                    this.promptBlock.Inlines.Clear();
                    this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    this.promptBlock.Inlines.Add(new Run() { Text = leftText + "\"" + this.completion + "\"" });
                }
                else
                {
                    this.promptBlock.Inlines.Clear();
                    this.promptBlock.Inlines.AddRange(this.GetPrompt(this.Prompt));
                    this.promptBlock.Inlines.Add(new Run() { Text = leftText + this.completion, });
                }
                this.inputText = inputText;
            }
        }

        private static void PromptPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self)
            {
                self.RefreshPrompt();
            }
        }

        private static object PromptPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            return baseValue ?? string.Empty;
        }

        private static void OutputForegroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self && self.textBox != null)
            {
                self.isChanged = true;
            }
        }

        private static void OutputBackgroundPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self && self.textBox != null)
            {
                self.isChanged = true;
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.refStack > 0)
                return;

            this.inputText = this.Command;
            this.completion = string.Empty;
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Home)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {
                    e.Handled = true;
                    this.MoveToFirst();
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    e.Handled = true;
                    var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
                    this.textBox.Selection.Select(commandStart, this.textBox.Selection.Start);
                }
            }
            else if (e.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.CursorPosition < 0)
                {
                    this.MoveToLast();
                    e.Handled = true;
                }
                else if (this.Command != string.Empty)
                {
                    this.Command = string.Empty;
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Enter && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    try
                    {
                        this.Execute();
                    }
                    finally
                    {
                        e.Handled = true;
                    }
                }
            }
            else if (e.Key == Key.Tab)
            {
                if (Keyboard.Modifiers == ModifierKeys.None)
                {

                    e.Handled = true;
                    this.NextCompletion();
                }
                else if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    e.Handled = true;
                    this.PrevCompletion();
                }
            }
            else if (e.Key == Key.Back && Keyboard.Modifiers == ModifierKeys.None)
            {
                e.Handled = this.CursorPosition <= 0;
            }
            else if (e.Key == Key.Up && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    this.PrevHistory();
                    e.Handled = true;
                }
            }
            else if (e.Key == Key.Down && Keyboard.Modifiers == ModifierKeys.None)
            {
                if (this.textBox.IsReadOnly == false)
                {
                    this.NextHistory();
                    e.Handled = true;
                }
            }
        }

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var selection = this.textBox.Selection;
            var pointer = this.GetPointer(0);
            if (pointer != null && (selection.Start.CompareTo(pointer) < 0 || selection.End.CompareTo(pointer) < 0))
            {
                this.textBox.IsReadOnly = true;
            }
            else
            {
                this.textBox.IsReadOnly = false;
            }
        }

        private void RefreshPrompt()
        {
            if (this.textBox != null)
            {
                this.refStack++;
                this.Clear();
                this.refStack--;
            }
        }

        private void AppendInternal(string text)
        {
            this.refStack++;
            try
            {
                var isEnd = this.textBox.CaretPosition.CompareTo(this.promptBlock.ContentEnd) == 0;
                if (this.output == null || this.isChanged == true)
                {
                    var oldOutput = this.output;
                    this.output = new Run();
                    if (this.OutputForeground != null)
                        this.output.Foreground = this.OutputForeground;
                    if (this.OutputBackground != null)
                        this.output.Background = this.OutputBackground;
                    if (oldOutput == null)
                    {
                        this.outputBlock = new Paragraph(this.output);
                        this.textBox.Document.Blocks.InsertBefore(this.promptBlock, this.outputBlock);
                    }
                    else
                    {
                        this.outputBlock.Inlines.InsertAfter(oldOutput, this.output);
                    }
                }
                if (text.EndsWith(Environment.NewLine) == true)
                {
                    text = text.Substring(0, text.Length - Environment.NewLine.Length);
                    this.output.Text += this.outputLeftText + text;
                    this.outputLeftText = Environment.NewLine;
                }
                else
                {
                    this.output.Text += this.outputLeftText + text;
                    this.outputLeftText = string.Empty;
                }
                if (isEnd == true)
                    this.textBox.CaretPosition = this.promptBlock.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        private TextPointer GetPointer(int cursorPosition)
        {
            var prompt = this.Prompt;
            var pointer = this.promptBlock.ContentStart;
            var contentStart = this.promptBlock.ContentStart;
            var position = prompt.Length + cursorPosition;
            while (pointer != null)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    var length = pointer.GetTextRunLength(LogicalDirection.Forward);
                    if (position <= length)
                    {
                        return pointer.GetPositionAtOffset(position, LogicalDirection.Forward);
                    }
                    position -= length;
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }
            return null;
        }

        private void SetCursorPosition(int cursorPosition)
        {
            if (this.GetPointer(cursorPosition) is TextPointer pointer)
            {
                this.textBox.CaretPosition = pointer;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
