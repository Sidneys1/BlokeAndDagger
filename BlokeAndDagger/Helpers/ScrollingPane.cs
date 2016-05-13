using System;
using System.Collections.Generic;
using System.Linq;

namespace BlokeAndDagger.Helpers {

	internal class ScrollingPane {
		#region Fields

		private readonly List<FormattedString> _entries = new List<FormattedString>();

		#endregion Fields

		#region Properties

		public IEnumerable<FormattedString> Entries => _entries;

		public int Height { get; }

		public int Width { get; }

		public int X { get; }

		public int Y { get; }

		#endregion Properties

		#region Constructors

		public ScrollingPane(int x, int y, int width, int height) {
			Width = width;
			Height = height;
			X = x;
			Y = y;
			_entries.Add(new FormattedString());
		}

		#endregion Constructors

		#region Methods

		public ExtendedConsole.Coord Draw(bool redraw = false) {
			var numToDraw = 0;
			var space = Height;
			foreach (var height in Entries.Reverse().Select(entry =>
				entry.Length == 0 ? 1 : (entry.Length + Width - 1) / Width
				)) {
				space -= height;
				numToDraw++;
				if (space <= 0)
					break;
			}

			Console.ResetColor();

			var y = Y + Height - 1 - (space < 0 ? 0 : space);
			var first = false;
			var toret = new ExtendedConsole.Coord();
			foreach (var entry in Entries.Reverse().Take(numToDraw)) {
				var secs = entry.Length == 0 ? 1 : (entry.Length + Width - 1) / Width;
				var iy = y - (secs - 1);
				Console.SetCursorPosition(X, iy);
				foreach (var c in entry.GetToPrint()) {
					var line = c;
					while (Console.CursorLeft + line.Length > X + Width) {
						iy++;
						var over = (Console.CursorLeft + line.Length) - (X + Width);
						if (Console.CursorTop >= Y)
							Console.Write(line.Substring(0, line.Length - over));
						else
							Console.SetCursorPosition(Console.CursorLeft + (line.Length - over), Console.CursorTop);
						line = line.Substring(line.Length - over);
						Console.SetCursorPosition(X, iy);
					}
					if (Console.CursorTop >= Y)
						Console.Write(line);
					else
						Console.SetCursorPosition(Console.CursorLeft + line.Length, Console.CursorTop);
					Console.ResetColor();
				}
				if (!first) {
					toret.X = (short) Console.CursorLeft;
					toret.Y = (short) Console.CursorTop;
					first = true;
				}
				if (Console.CursorTop >= Y && Console.CursorLeft < (X + Width))
					Console.Write(new string(' ', (X + Width) - Console.CursorLeft));
				y -= secs;
			}

			Console.ResetColor();
			return toret;
		}

		public void Write(string str, ConsoleColor? f = null, ConsoleColor? b = null, bool r = false) {
			Write(new FormattedText(str, f, b, r));
		}

		public void Write(FormattedString str) {
			_entries[_entries.Count - 1].Append(str);
		}

		public void Write(FormattedText str) {
			_entries[_entries.Count - 1].Sections.Add(str);
		}

		public void WriteLine() {
			_entries.Add(new FormattedString());
		}

		public void WriteLine(string str, ConsoleColor? f = null, ConsoleColor? b = null, bool r = false) {
			Write(str, f, b, r);
			_entries.Add(new FormattedString());
		}

		public void WriteLine(FormattedString str) {
			Write(str);
			_entries.Add(new FormattedString());
		}

		public void WriteLine(FormattedText str) {
			Write(str);
			_entries.Add(new FormattedString());
		}

		#endregion Methods
	}
}