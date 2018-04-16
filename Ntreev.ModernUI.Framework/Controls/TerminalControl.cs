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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Ntreev.ModernUI.Framework.Controls
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(RichTextBox))]
    public class TerminalControl : Control
    {
        public readonly static DependencyProperty PromptProperty =
            DependencyProperty.Register(nameof(Prompt), typeof(string), typeof(TerminalControl),
                new PropertyMetadata(string.Empty, PromptPropertyChangedCallback, PromptPropertyCoerceValueCallback));

        private readonly static DependencyPropertyKey TextPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(Text), typeof(string), typeof(TerminalControl),
                new PropertyMetadata(string.Empty));
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
                this.promptBlock.Inlines.Add(new Run() { Text = this.Prompt, });
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
                var commandText = this.CommandText;
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
                this.promptBlock.Inlines.Add(new Run() { Text = this.Prompt, });
                this.inputText = string.Empty;
                this.completion = string.Empty;
                this.OnExecuted(new RoutedEventArgs(ExecutedEvent));
                this.textBox.CaretPosition = this.promptBlock.ContentEnd;
            }
            finally
            {
                this.refStack--;
            }
        }

        public void Clear()
        {
            this.promptBlock.Inlines.Clear();
            this.promptBlock.Inlines.Add(new Run() { Text = this.Prompt, });
            this.inputText = string.Empty;
            this.completion = string.Empty;
        }

        public void MoveToFirst()
        {
            var contentStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
            if (contentStart == null)
                this.textBox.CaretPosition = this.promptBlock.ContentEnd;
            else
                this.textBox.CaretPosition = contentStart;
        }

        public void MoveToLast()
        {
            this.textBox.CaretPosition = this.promptBlock.Inlines.LastInline.ContentEnd;
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
                this.promptBlock.Inlines.Add(new Run() { Text = this.Prompt, });
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
            get { return (string)this.GetValue(PromptProperty); }
            set { this.SetValue(PromptProperty, value); }
        }

        public Brush OutputForeground
        {
            get { return (Brush)this.GetValue(OutputForegroundProperty); }
            set { this.SetValue(OutputForegroundProperty, value); }
        }

        public Brush OutputBackground
        {
            get { return (Brush)this.GetValue(OutputBackgroundProperty); }
            set { this.SetValue(OutputBackgroundProperty, value); }
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
                this.inputText = this.CommandText = this.histories[this.historyIndex + 1];
                this.MoveToLast();
                this.historyIndex++;
            }
        }

        public void PrevHistory()
        {
            if (this.historyIndex > 0)
            {
                this.inputText = this.CommandText = this.histories[this.historyIndex - 1];
                this.MoveToLast();
                this.historyIndex--;
            }
            else if (this.histories.Count == 1)
            {
                this.inputText = this.CommandText = this.histories[0];
                this.MoveToLast();
                this.historyIndex = 0;
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
                    this.promptBlock.Inlines.Add(new Run()
                    {
                        Text = this.Prompt + leftText + "\"" + this.completion + "\"",
                    });
                }
                else
                {
                    this.promptBlock.Inlines.Clear();
                    this.promptBlock.Inlines.Add(new Run()
                    {
                        Text = this.Prompt + leftText + this.completion,
                    });
                }
                this.inputText = inputText;
            }
        }

        private static void PromptPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TerminalControl self)
            {
                self.SetPrompt(e.NewValue as string);
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

            this.inputText = this.CommandText;
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
                if (this.textBox.IsReadOnly == true)
                {
                    this.MoveToFirst();
                    e.Handled = true;
                }
                else
                {
                    //this.Clear();
                    //e.Handled = true;
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
                var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
                if (this.textBox.CaretPosition.CompareTo(commandStart) <= 0)
                {
                    e.Handled = true;
                }
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
            var commandStart = this.promptBlock.Inlines.FirstInline.ContentStart.GetPositionAtOffset(this.Prompt.Length);
            if (commandStart != null)
            {
                if (this.textBox.Selection.Start.CompareTo(commandStart) < 0 || this.textBox.Selection.End.CompareTo(commandStart) < 0)
                {
                    this.textBox.IsReadOnly = true;
                }
                else
                {
                    this.textBox.IsReadOnly = false;
                }
            }
            else
            {
                this.textBox.IsReadOnly = false;
            }
        }

        private void SetPrompt(string prompt)
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
            }
            finally
            {
                this.refStack--;
            }
        }

        private string CommandText
        {
            get
            {
                var text = string.Empty;
                foreach (var item in this.promptBlock.Inlines)
                {
                    if (item is Run run)
                    {
                        text += run.Text;
                    }
                }
                return text.Substring(this.Prompt.Length);
            }
            set
            {
                this.refStack++;
                try
                {
                    this.promptBlock.Inlines.Clear();
                    this.promptBlock.Inlines.Add(new Run()
                    {
                        Text = this.Prompt + value,
                    });
                }
                finally
                {
                    this.refStack--;
                }
            }
        }
    }
}
