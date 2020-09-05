#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using static System.Math;
using Arc = PathStepArc;
using Line = PathStepLine;


Console.WriteLine(Design5());

string Design4()
{
    var circleRadius = 9.5;
    var circleCenter = new RadialPoint(185, 91.5 - circleRadius);
    var circleBoundary = 13;
    var circleOuterRadius = RadialPoint.TangentialAt(new(210 - circleBoundary, 89), -90 + circleBoundary + 5, circleCenter).Radius;
    var hingePoints = new[] { new RadialPoint(-20, 50), new RadialPoint(110, 50) };
    var hingeNormal = (hingePoints[1] - hingePoints[0]).Normalize();
    var hingePerpendicular = new CoordinatePoint(X: hingeNormal.Y, Y: -hingeNormal.X);
    var spaceDegrees = 6;
    return string.Join(" ", new[] {
        new PathBuilder(new RadialPoint(30, 64.5))
            .AddStep(new Arc(new (-30, 64.5)))
            .AddStep(new Line(new RadialPoint(-30, 91.5)))
            .AddStep(new Arc(new (30, 91.5))),
        new PathBuilder(new RadialPoint(160, 50))
            .AddStep(new Line(new RadialPoint(290, 50)))
            .AddStep(new Arc(new (210, 50)))
            .AddStep(new Line(new RadialPoint(210, 89)))
            .AddStep(new Arc(new(210 - circleBoundary, 89)))
            .AddStep(new Line(new RadialPoint(-90 + circleBoundary + 5, circleOuterRadius, circleCenter)))
            .AddStep(new Arc(new RadialPoint(90 - circleBoundary + 5, circleOuterRadius, circleCenter)))
            .AddStep(new Line(new RadialPoint(160 + circleBoundary, 89)))
            .AddStep(new Arc(new(160, 89))),

        new PathBuilder(new RadialPoint(160 - spaceDegrees, 50))
            .AddStep(new Arc(new (135 + spaceDegrees / 2, 50)))
            .AddStep(new Line(new RadialPoint(315 - spaceDegrees / 2, 50)))
            .AddStep(new Arc(new RadialPoint(290 + spaceDegrees, 50))),

        new PathBuilder(new RadialPoint(110 + spaceDegrees, 50))
            .AddStep(new Arc(new (135 - spaceDegrees / 2, 50)))
            .AddStep(new Line(new RadialPoint(315 + spaceDegrees / 2, 50)))
            .AddStep(new Arc(new RadialPoint(340 - spaceDegrees, 50))),

        new PathBuilder(new RadialPoint(0, circleRadius, circleCenter))
            .AddStep(new Arc(new (180, circleRadius, circleCenter)))
            .AddStep(new Arc(new (360, circleRadius, circleCenter))),

        new PathBuilder(hingePoints[0])
            .AddStep(new Arc(hingePoints[1]))
            .AddStep(new Line(hingePoints[1] - hingeNormal * 7))
            .AddStep(new Line(hingePoints[1] - hingeNormal * 7 - hingePerpendicular * 2))
            .AddStep(new Line((hingePoints[0] + hingePoints[1]) / 2 - hingePerpendicular * 2))
            .AddStep(new Line((hingePoints[0] + hingePoints[1]) / 2))
    }.Select(builder => builder.Build()));

}

string Design5()
{
    return string.Join(" ", new[] {
        new PathBuilder(new RadialPoint(-45, 53.5))
            .AddStep(new Arc(new (-30, 53.5)))
            .AddStep(new Line(new RadialPoint (-30, 91.5)))
            .AddStep(new Arc(new (5, 91.5)))
            .AddStep(new Line(new RadialPoint (5, 87)))
            .AddStep(new Arc(new (20, 87)))
            .AddStep(new Line(new RadialPoint (20, 91.5)))
            .AddStep(new Arc(new RadialPoint (30, 91.5)))
            .AddStep(new Line(new RadialPoint (30, 53.5)))
            .AddStep(new Arc(new (135, 53.5)))
            .AddStep(new Line(new RadialPoint(135, 10)))
            .AddStep(new Line(new RadialPoint(-45, -4.35) + new RadialPoint(45, -8, new CoordinatePoint(0, 0))))
            .AddStep(new Line(new RadialPoint(-45, 22.627) + new RadialPoint(45, -8, new CoordinatePoint(0, 0))))
            .AddStep(new Line(new RadialPoint(-45, 25.456) + new RadialPoint(45, -4, new CoordinatePoint(0, 0))))
            .AddStep(new Line(new RadialPoint(-45, 35.348) + new RadialPoint(45, -4, new CoordinatePoint(0, 0))))
            .AddStep(new Line(new RadialPoint(-45, 38.177) + new RadialPoint(45, -8, new CoordinatePoint(0, 0))))
            .AddStep(new Line(new RadialPoint(-45, 53.5) + new RadialPoint(45, -8, new CoordinatePoint(0, 0)))),

        // arc overlapping arm
        new PathBuilder(new RadialPoint(135, 63))
            .AddStep(new Arc(new (180, 63)))
            .AddStep(new Line(new RadialPoint(180, 91.5)))
            .AddStep(new Arc(new (135, 91.5))),

        // small arcs
        new PathBuilder(new RadialPoint(185, 63))
            .AddStep(new Arc(new (200, 63)))
            .AddStep(new Line(new RadialPoint(200, 69)))
            .AddStep(new Arc(new (185, 69))),
        new PathBuilder(new RadialPoint(185, 75))
            .AddStep(new Arc(new (200, 75)))
            .AddStep(new Line(new RadialPoint(200, 81)))
            .AddStep(new Arc(new (185, 81))),

        // top arc
        new PathBuilder(new RadialPoint(180+63, 63))
            .AddStep(new Arc(new (360-63, 63)))
            .AddStep(new Line(new RadialPoint(360-63, 91.5)))
            .AddStep(new Arc(new (180+63, 91.5))),

    }.Select(builder => builder.Build()));

}

string Design5Inset()
{
    return string.Join(" ", new[] {
        new PathBuilder(new RadialPoint(0, 13, new CoordinatePoint(77, 123)))
            .AddStep(new Arc(new (180, 13, new CoordinatePoint(77, 123))))
            .AddStep(new Arc(new (360, 13, new CoordinatePoint(77, 123)))),

        new PathBuilder(new RadialPoint(360, 53.5))
            .AddStep(new Arc(new (180, 53.5)))
            .AddStep(new Line(new RadialPoint(180, 32.5, new CoordinatePoint(87, 100))))
            .AddStep(new Arc(new RadialPoint(360, 32.5, new CoordinatePoint(87, 100))))
            .AddStep(new Line(new RadialPoint(360, 42.5, new CoordinatePoint(94, 100))))
            .AddStep(new Arc(new RadialPoint(300, 42.5, new CoordinatePoint(94, 100))))
            .AddStep(new Line(new RadialPoint(300, 48, new CoordinatePoint(94, 100))))
            .AddStep(new Arc(new RadialPoint(360, 48, new CoordinatePoint(94, 100)))),

    }.Select(builder => builder.Build()));

}

string Design6()
{
    var points = new[] {
        new CoordinatePoint(52, 33),
        new CoordinatePoint(46, 23),
        new CoordinatePoint(34, 23),
        new CoordinatePoint(34, -23),
        new CoordinatePoint(46, -23),
        new CoordinatePoint(52, -33),
    };
    return string.Join(" ", new[] {
        new PathBuilder(new RadialPoint(-32, 91.5))
            .AddStep(new Arc(new (32, 91.5)))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[0]))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[1]))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[2]))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[3]))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[4]))
            .AddStep(new Line(new CoordinatePoint(100, 100) + points[5])),

        new PathBuilder(new RadialPoint(-32, -91.5))
            .AddStep(new Arc(new (32, -91.5)))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[0]))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[1]))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[2]))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[3]))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[4]))
            .AddStep(new Line(new CoordinatePoint(100, 100) - points[5])),

        new PathBuilder(new CoordinatePoint(75.5,51))
            .AddStep(new Line(new CoordinatePoint(75.5, 149)))
            .AddStep(new Line(new CoordinatePoint(87.5, 149)))
            .AddStep(new Line(new CoordinatePoint(87.5, 51))),

        new PathBuilder(new CoordinatePoint(94,51))
            .AddStep(new Line(new CoordinatePoint(94, 149)))
            .AddStep(new Line(new CoordinatePoint(106, 149)))
            .AddStep(new Line(new CoordinatePoint(106, 51))),

        new PathBuilder(new CoordinatePoint(112.5,51))
            .AddStep(new Line(new CoordinatePoint(112.5, 77)))
            .AddStep(new Line(new CoordinatePoint(114, 80)))
            .AddStep(new Line(new CoordinatePoint(114, 149)))
            .AddStep(new Line(new CoordinatePoint(123, 149)))
            .AddStep(new Line(new CoordinatePoint(123, 80)))
            .AddStep(new Line(new CoordinatePoint(124.5, 77)))
            .AddStep(new Line(new CoordinatePoint(124.5, 51))),
        /*
        <path d="M112.5,51 h12 v19 l-2,6 v73 h-8 v-73 l-2,-6 Z" />
*/

        new PathBuilder(new CoordinatePoint(128,110))
            .AddStep(new Line(new CoordinatePoint(128, 123)))
            .AddStep(new Line(new CoordinatePoint(131, 123)))
            .AddStep(new Line(new CoordinatePoint(131, 110))),
    }.Select(builder => builder.Build()));

}

string Design6Inset()
{
    var circleDegrees = -22.5;
    return string.Join(" ", new[] {
        new PathBuilder(new RadialPoint(0, 7, new RadialPoint(circleDegrees, -90)))
            .AddStep(new Arc(new (180, 7, new RadialPoint(circleDegrees, -90))))
            .AddStep(new Arc(new (360, 7, new RadialPoint(circleDegrees, -90)))),
        new PathBuilder(new RadialPoint(0, 7, new RadialPoint(circleDegrees, 90)))
            .AddStep(new Arc(new (180, 7, new RadialPoint(circleDegrees, 90))))
            .AddStep(new Arc(new (360, 7, new RadialPoint(circleDegrees, 90)))),

    }.Select(builder => builder.Build()));

}


public abstract record Point
{
    public abstract CoordinatePoint Coordinates();
    public abstract string ToSvgPoint();
    public static CoordinatePoint operator +(Point a, Point b) => a.Coordinates() + b.Coordinates();
    public static CoordinatePoint operator -(Point a, Point b) => a.Coordinates() - b.Coordinates();
    public static CoordinatePoint operator *(Point a, double scalar) => a.Coordinates() * scalar;
    public static CoordinatePoint operator /(Point a, double scalar) => a.Coordinates() / scalar;
    public virtual double Distance() => this.Coordinates().Distance();
    public virtual CoordinatePoint Normalize() => this.Coordinates().Normalize();
}
public record CoordinatePoint(double X, double Y) : Point
{
    public static CoordinatePoint operator +(CoordinatePoint a, CoordinatePoint b) => new(a.X + b.X, a.Y + b.Y);
    public static CoordinatePoint operator -(CoordinatePoint a, CoordinatePoint b) => new(a.X - b.X, a.Y - b.Y);
    public static CoordinatePoint operator *(CoordinatePoint a, double scalar) => new(a.X * scalar, a.Y * scalar);
    public static CoordinatePoint operator /(CoordinatePoint a, double scalar) => new(a.X / scalar, a.Y / scalar);
    public override CoordinatePoint Coordinates() => this;
    public override string ToSvgPoint() => $"{X:0.000000},{Y:0.000000}";
    public override double Distance() => Sqrt(X * X + Y * Y);
    public override CoordinatePoint Normalize() => this / Distance();
}
public record RadialPoint : Point
{
    const double Tau = PI * 2;
    const double Deg2Rad = Tau / 360;
    public static readonly CoordinatePoint defaultOrigin = new(100, 100);

    public RadialPoint(double Degrees, double Radius, Point? Origin = default)
    {
        this.Degrees = Degrees;
        this.Radius = Radius;
        this.Origin = Origin ?? defaultOrigin;
    }

    public void Deconstruct(out double Degrees, out double Radius, out Point Origin)
    {
        Degrees = this.Degrees;
        Radius = this.Radius;
        Origin = this.Origin;
    }

    public double Degrees { get; }
    public double Radius { get; }
    public Point Origin { get; }

    public override CoordinatePoint Coordinates() => new CoordinatePoint(Cos(Degrees * Deg2Rad), Sin(Degrees * Deg2Rad)) * Radius + Origin;
    public override string ToSvgPoint() => Coordinates().ToSvgPoint();

    public static RadialPoint TangentialAt(RadialPoint radial, double degrees, Point? origin = default)
    {
        var newOrigin = origin ?? defaultOrigin;

        // A = newOrigin
        // B = new point = 90
        // C = radial.Origin

        var distanceAC = (newOrigin - radial.Origin).Distance();
        var triangleRadC = radial.Degrees * Deg2Rad - Atan2((newOrigin - radial.Origin).Y, (newOrigin - radial.Origin).X);
        // c / sin C = b / sin B
        // c / sin C = distanceAC / 1
        // c = distanceAC * sin C
        var radius = Math.Abs(distanceAC * Sin(triangleRadC));

        return new(degrees, radius, newOrigin);
    }

}
public abstract record PathStep(Point Point)
{
    public abstract string ToSvgPathInstruction(Point currentPoint);
}
public record PathStepLine : PathStep
{
    public PathStepLine(Point Point) : base(Point) { }

    public override string ToSvgPathInstruction(Point currentPoint) => $"L{Point.ToSvgPoint()}";
}
public record PathStepArc : PathStep
{
    public PathStepArc(RadialPoint Point) : base(Point) { }

    public override string ToSvgPathInstruction(Point currentPoint) =>
        currentPoint switch
        {
            RadialPoint(double initialDegrees, double initialRadius, Point initialOrigin) => Point switch
            {
                RadialPoint(double endDegrees, double endRadius, Point endOrigin) when initialRadius == endRadius && initialOrigin == endOrigin =>
                    $"A{initialRadius:0.000000},{initialRadius:0.000000} 0 {(Abs(endDegrees - initialDegrees) > 180 ? "1" : "0")} {(endDegrees > initialDegrees ? "1" : "0")} {Point.ToSvgPoint()}",
                _ => throw new NotSupportedException("Different start/end radius on arc")
            },
            _ => throw new NotSupportedException("Must start an arc with a radial point")
        };
}
public record PathBuilder(Point startPoint)
{
    public ImmutableList<PathStep> Path { get; private init; } = ImmutableList<PathStep>.Empty;

    public PathBuilder AddStep(PathStep step)
    {
        return this with { Path = Path.Add(step) };
    }

    public string Build()
    {
        var resultBuilder = Path.Aggregate(
            (builder: new StringBuilder($"M{startPoint.ToSvgPoint()} "), currentPoint: startPoint),
            (prev, next) => (
                builder: prev.builder.Append($"{next.ToSvgPathInstruction(prev.currentPoint)} "),
                currentPoint: next.Point
            )).builder;
        return resultBuilder.Append("Z").ToString();
    }
}
