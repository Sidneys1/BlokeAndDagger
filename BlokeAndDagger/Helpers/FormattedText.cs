using System;

namespace BlokeAndDagger.Helpers {

	public struct FormattedText {
		#region Fields

		public ConsoleColor? BackgroundColor;
		public ConsoleColor? ForegroundColor;
		public bool Reset;
		public string Text;

		#endregion Fields

		#region Constructors

		public FormattedText(string text, ConsoleColor? fcolor = null, ConsoleColor? bcolor = null, bool reset = false) {
			Text = text;
			ForegroundColor = fcolor;
			BackgroundColor = bcolor;
			Reset = reset;
		}

		#endregion Constructors

		#region Methods

		public static explicit operator string(FormattedText s) => s.Text;

		public static implicit operator FormattedText(string s) => new FormattedText(s, reset: true);

		public void SetColors() {
			if (Reset)
				Console.ResetColor();
			else {
				if (ForegroundColor.HasValue)
					Console.ForegroundColor = ForegroundColor.Value;
				if (BackgroundColor.HasValue)
					Console.BackgroundColor = BackgroundColor.Value;
			}
		}

		public override string ToString() => Text;

		public void Write() {
			SetColors();
			Console.Write(Text);
		}

		#endregion Methods
	}
}