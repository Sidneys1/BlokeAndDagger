using System;

namespace BlokeAndDagger.Helpers {
    public sealed class ConsoleColorAttribute : Attribute {
        public ConsoleColor ForegroundColor { get; }
        public ConsoleColor BackgroundColor { get; }

        public ConsoleColorAttribute(ConsoleColor foregroundColor = ConsoleColor.DarkGray,
            ConsoleColor backgroundColor = ConsoleColor.Black) {
            ForegroundColor = foregroundColor;
            BackgroundColor = backgroundColor;
        }

        public void SetColors() {
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class WeightAttribute : Attribute
    {
        public WeightAttribute(double weight)
        {
            Weight = weight;
        }
        public double Weight { get; }
    }
}
