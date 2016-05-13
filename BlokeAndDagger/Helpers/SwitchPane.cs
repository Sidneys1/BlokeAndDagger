using System;
using System.Collections.Generic;
using System.Linq;

namespace BlokeAndDagger.Helpers {

	internal class SwitchPane {
		#region Properties

		public int Width { get; }
		public int X { get; }
		public int Y { get; }

		#endregion Properties

		#region Constructors

		public SwitchPane(int x, int y, int w) {
			X = x;
			Y = y;
			Width = w;
		}

		#endregion Constructors

		#region Methods

		public void Draw(FormattedString header, FormattedString post, IEnumerable<FormattedString> options) {
			var opts = options.ToArray();
			var space = Width - header.Length;
			var maxl = space;

			ExtendedConsole.ClearConsoleLine((short)Y);

			while (opts.Select(o => o.Length + post.Length > maxl ? o.ToString().Substring(0, Math.Min(o.Length, maxl)) + post : o).Distinct().Sum(o => o.Length + 2) - 2 > space) {
				maxl--;
				if (maxl <= 0)
					return;
			}

			Console.SetCursorPosition(X, Y);
			var x = opts.GroupBy(o => o.Length + post.Length > maxl ? o.ToString().Substring(0, Math.Min(o.Length, maxl)).White() + post : o);
			var str = x.Select(o => o.First()).Aggregate(header, (s, formattedString) => s + formattedString + ", ".DarkGray());
			str.Sections.RemoveAt(str.Sections.Count - 1);
			str.Write();
		}

		#endregion Methods
	}
}