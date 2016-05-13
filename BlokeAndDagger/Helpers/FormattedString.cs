using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlokeAndDagger.Helpers {

	public class FormattedString {
		#region Properties

		public int Length => Sections.Sum(o => o.Text.Length);

		public List<FormattedText> Sections { get; } = new List<FormattedText>();

		#endregion Properties

		#region Constructors

		public FormattedString(params FormattedText[] sections) {
			Sections.AddRange(sections);
		}

		public FormattedString(FormattedText section) {
			Sections.Add(section);
		}

		#endregion Constructors

		#region Methods

		public static implicit operator FormattedString(string str) => new FormattedString(str);

		public static implicit operator FormattedString(FormattedText str) => new FormattedString(str);

		public static FormattedString Join(FormattedString s, IEnumerable<FormattedString> @select) {
			var b = new FormattedString();
			foreach (var formattedString in select) {
				b.Append(formattedString);
				b.Append(s);
			}
			if (b.Sections.Count > 1)
				b.Sections.RemoveAt(b.Sections.Count - 1);
			return b;
		}

		public static FormattedString operator +(FormattedString left, FormattedString right) {
			FormattedString ret = new FormattedString(left.Sections.ToArray());
			ret.Sections.AddRange(right.Sections);
			return ret;
		}

		public void Append(FormattedString str) {
			Sections.AddRange(str.Sections);
		}

		public IEnumerable<string> GetToPrint() {
			foreach (var section in Sections) {
				section.SetColors();
				yield return section.Text;
			}
		}

		public bool StartsWith(string entry, StringComparison strCmp) => ToString().StartsWith(entry, strCmp);

		public FormattedString Substring(int i) {
			var ret = new FormattedString();

			ConsoleColor? currentForeground = null;
			ConsoleColor? currentBackground = null;
			bool reset = false;
			bool set = false;
			foreach (var formattedText in Sections) {
				if (formattedText.Text.Length <= i) {
					i -= formattedText.Text.Length;
					currentForeground = formattedText.ForegroundColor ?? currentForeground;
					currentBackground = formattedText.BackgroundColor ?? currentBackground;
					reset |= formattedText.Reset;
					continue;
				}
				if (!set) {
					ret.Sections.Add(new FormattedText(string.Empty, currentForeground, currentBackground, reset));
					set = true;
				}

				ret.Sections.Add(formattedText);
			}

			return ret;
		}

		public override string ToString() =>
																			Sections.Aggregate(new StringBuilder(), (builder, text) => builder.Append(text.Text)).ToString();

		public void Write() {
			foreach (var sec in GetToPrint())
				Console.Write(sec);
		}

		#endregion Methods
	}
}