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
using System.Windows;

namespace Ntreev.ModernUI.Framework.Controls
{
    public static class TerminalColors
    {
        public readonly static ComponentResourceKey ForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(ForegroundKey));
        public readonly static ComponentResourceKey BackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(BackgroundKey));

        public readonly static ComponentResourceKey BlackForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(BlackForegroundKey));
        public readonly static ComponentResourceKey DarkBlueForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkBlueForegroundKey));
        public readonly static ComponentResourceKey DarkGreenForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkGreenForegroundKey));
        public readonly static ComponentResourceKey DarkCyanForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkCyanForegroundKey));
        public readonly static ComponentResourceKey DarkRedForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkRedForegroundKey));
        public readonly static ComponentResourceKey DarkMagentaForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkMagentaForegroundKey));
        public readonly static ComponentResourceKey DarkYellowForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkYellowForegroundKey));
        public readonly static ComponentResourceKey GrayForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(GrayForegroundKey));
        public readonly static ComponentResourceKey DarkGrayForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkGrayForegroundKey));
        public readonly static ComponentResourceKey BlueForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(BlueForegroundKey));
        public readonly static ComponentResourceKey GreenForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(GreenForegroundKey));
        public readonly static ComponentResourceKey CyanForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(CyanForegroundKey));
        public readonly static ComponentResourceKey RedForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(RedForegroundKey));
        public readonly static ComponentResourceKey MagentaForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(MagentaForegroundKey));
        public readonly static ComponentResourceKey YellowForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(YellowForegroundKey));
        public readonly static ComponentResourceKey WhiteForegroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(WhiteForegroundKey));

        public readonly static ComponentResourceKey BlackBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(BlackBackgroundKey));
        public readonly static ComponentResourceKey DarkBlueBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkBlueBackgroundKey));
        public readonly static ComponentResourceKey DarkGreenBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkGreenBackgroundKey));
        public readonly static ComponentResourceKey DarkCyanBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkCyanBackgroundKey));
        public readonly static ComponentResourceKey DarkRedBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkRedBackgroundKey));
        public readonly static ComponentResourceKey DarkMagentaBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkMagentaBackgroundKey));
        public readonly static ComponentResourceKey DarkYellowBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkYellowBackgroundKey));
        public readonly static ComponentResourceKey GrayBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(GrayBackgroundKey));
        public readonly static ComponentResourceKey DarkGrayBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(DarkGrayBackgroundKey));
        public readonly static ComponentResourceKey BlueBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(BlueBackgroundKey));
        public readonly static ComponentResourceKey GreenBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(GreenBackgroundKey));
        public readonly static ComponentResourceKey CyanBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(CyanBackgroundKey));
        public readonly static ComponentResourceKey RedBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(RedBackgroundKey));
        public readonly static ComponentResourceKey MagentaBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(MagentaBackgroundKey));
        public readonly static ComponentResourceKey YellowBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(YellowBackgroundKey));
        public readonly static ComponentResourceKey WhiteBackgroundKey = new ComponentResourceKey(typeof(TerminalControl), nameof(WhiteBackgroundKey));

        public static ComponentResourceKey FindForegroundKey(ConsoleColor? consoleColor)
        {
            if (consoleColor.HasValue == false)
                return ForegroundKey;
            return consoleColor.Value switch
            {
                ConsoleColor.Black => BlackForegroundKey,
                ConsoleColor.DarkBlue => DarkBlueForegroundKey,
                ConsoleColor.DarkGreen => DarkGreenForegroundKey,
                ConsoleColor.DarkCyan => DarkCyanForegroundKey,
                ConsoleColor.DarkRed => DarkRedForegroundKey,
                ConsoleColor.DarkMagenta => DarkMagentaForegroundKey,
                ConsoleColor.DarkYellow => DarkYellowForegroundKey,
                ConsoleColor.Gray => GrayForegroundKey,
                ConsoleColor.DarkGray => DarkGrayForegroundKey,
                ConsoleColor.Blue => BlueForegroundKey,
                ConsoleColor.Green => GreenForegroundKey,
                ConsoleColor.Cyan => CyanForegroundKey,
                ConsoleColor.Red => RedForegroundKey,
                ConsoleColor.Magenta => MagentaForegroundKey,
                ConsoleColor.Yellow => YellowForegroundKey,
                ConsoleColor.White => WhiteForegroundKey,
                _ => throw new NotImplementedException(),
            };
        }

        public static ComponentResourceKey FindBackgroundKey(ConsoleColor? consoleColor)
        {
            if (consoleColor.HasValue == false)
                return BackgroundKey;
            switch (consoleColor.Value)
            {
                case ConsoleColor.Black: return BlackBackgroundKey;
                case ConsoleColor.DarkBlue: return DarkBlueBackgroundKey;
                case ConsoleColor.DarkGreen: return DarkGreenBackgroundKey;
                case ConsoleColor.DarkCyan: return DarkCyanBackgroundKey;
                case ConsoleColor.DarkRed: return DarkRedBackgroundKey;
                case ConsoleColor.DarkMagenta: return DarkMagentaBackgroundKey;
                case ConsoleColor.DarkYellow: return DarkYellowBackgroundKey;
                case ConsoleColor.Gray: return GrayBackgroundKey;
                case ConsoleColor.DarkGray: return DarkGrayBackgroundKey;
                case ConsoleColor.Blue: return BlueBackgroundKey;
                case ConsoleColor.Green: return GreenBackgroundKey;
                case ConsoleColor.Cyan: return CyanBackgroundKey;
                case ConsoleColor.Red: return RedBackgroundKey;
                case ConsoleColor.Magenta: return MagentaBackgroundKey;
                case ConsoleColor.Yellow: return YellowBackgroundKey;
                case ConsoleColor.White: return WhiteBackgroundKey;
            }
            throw new NotImplementedException();
        }
    }
}
