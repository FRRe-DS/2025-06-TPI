using System.ComponentModel.DataAnnotations;

namespace ApiDePapas.Domain.ValueObjects;

public class Coordinates
{
    [Required]
    [Range(-90.0, 90.0)]
    public float Lat { get; set; }

    [Required]
    [Range(-180.0, 180.0)]
    public float Lon { get; set; }

    public Coordinates(float lat, float lon)
    {
        Lat = lat;
        Lon = lon;
    }

    // Construir coordenadas utilizando un punto en una esfera unitaria
    public Coordinates(float x, float y, float z)
    {
        double hyp = Math.Sqrt(x * x + y * y);

        double _lat = Math.Atan2(z, hyp);
        double _lon = Math.Atan2(y, x);

        Lat = (float)Angle.RadToDeg(_lat);
        Lon = (float)Angle.RadToDeg(_lon);
    }
}

public static class Angle
{
    public static double DegToRad(double degrees) => degrees * Math.PI / 180.0f;
    public static double RadToDeg(double radians) => radians * 180.0f / Math.PI;
}