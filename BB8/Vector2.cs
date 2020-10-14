using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BB8
{
    public record Vector2(double X, double Y)
    {
        public double Dot(Vector2 other) => X * other.X + Y * other.Y;

        public Vector2 MaxUnit() =>
            (X * X + Y * Y) switch
            {
                < 1 => this,
                var norm => this.Multiply(1 / Math.Sqrt(norm))
            };

        public Vector2 Multiply(double factor) => new(X * factor, Y * factor);

        public override string ToString() => $"({X:0.00}, {Y:0.00})";
    }
}
