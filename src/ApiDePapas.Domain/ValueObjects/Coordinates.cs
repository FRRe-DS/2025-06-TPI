using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.ValueObjects;

public record Coordinates(
    [property: Required, Range(-90.0, 90.0)]
    float Lat,

    [property: Required, Range(-180.0, 180.0)]
    float Lon
)
{
    // Construir coordenadas utilizando un punto en una esfera unitaria
    public Coordinates(float x, float y, float z)
        : this(
            (float)Angle.RadToDeg(Math.Atan2(z, Math.Sqrt(x * x + y * y))),
            (float)Angle.RadToDeg(Math.Atan2(y, x))
        )
    { }
}

public record Angle(double Degrees)
{
    public double Radians => DegToRad(Degrees);

    public static Angle FromDegrees(double degrees)
    {
        return new Angle(DegToRad(degrees));
    }

    public static Angle FromRadians(double radians)
    {
        return new Angle(radians);
    }

    public static double DegToRad(double degrees) => degrees * Math.PI / 180.0f;
    public static double RadToDeg(double radians) => radians * 180.0f / Math.PI;
}