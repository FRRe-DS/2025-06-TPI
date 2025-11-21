using System.Numerics;

using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Repositories;
using ApiDePapas.Domain.ValueObjects;

// Es una mejor aproximación que DistanceServiceInMemory. Sirve para
// calcular distancias entre departamentos en lugar de provincias, por lo
// que usa los 5 primeros dígitos del código postal y promedia las coordenadas
// geográficas de las localidades con ese código postal.

// Puede utilizarse una API externa en el futuro si se quiere precisión a nivel
// calle/dirección.

namespace ApiDePapas.Application.Services;

public class DistanceServiceInternal : IDistanceService
{
    private readonly ILocalityRepository _localityRepository;
    
    public DistanceServiceInternal(ILocalityRepository localityRepository)
    {
        _localityRepository = localityRepository;
    }

    public async Task<float> GetDistanceKm(string originCpa, string destinationCpa)
    {
        var possibleOriginLocalities = await _localityRepository.GetByPostalCodeAsync(originCpa);
        var possibleDestinationLocalities = await _localityRepository.GetByPostalCodeAsync(destinationCpa);

        if (!possibleOriginLocalities.Any() || !possibleDestinationLocalities.Any()) { return 300.0f; } // fallback neutro

        List<Coordinates> possibleOriginCoords = possibleOriginLocalities
            .Select(l => new Coordinates(l.lat, l.lon))
            .ToList();

        List<Coordinates> possibleDestinationCoords = possibleDestinationLocalities
            .Select(l => new Coordinates(l.lat, l.lon))
            .ToList();

        Coordinates originCentroid = GetAverageCoordinates(possibleOriginCoords);
        Coordinates destinationCentroid = GetAverageCoordinates(possibleDestinationCoords);

        return HaversineKm(originCentroid, destinationCentroid);
    }

    private static float HaversineKm(Coordinates p1, Coordinates p2)
    {
        const double R = 6371.0;

        double dLat = Angle.DegToRad(p2.Lat - p1.Lat);
        double dLon = Angle.DegToRad(p2.Lon - p1.Lon);

        double hv = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(Angle.DegToRad(p1.Lat)) *
                    Math.Cos(Angle.DegToRad(p2.Lat)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(hv), Math.Sqrt(1 - hv));
        return (float)(R * c);
    }

    private static Coordinates GetAverageCoordinates(List<Coordinates> points)
    {
        if (points == null || points.Count == 0)
        {
            throw new ArgumentException("La lista de puntos no puede ser nula o vacía.");
        }

        var cartesianPoint = new Vector3(0.0f, 0.0f, 0.0f);

        foreach (Coordinates p in points)
        {
            double latRad = Angle.DegToRad(p.Lat);
            double lonRad = Angle.DegToRad(p.Lon);

            double cosLat = Math.Cos(latRad);

            cartesianPoint.X += (float)(cosLat * Math.Cos(lonRad));
            cartesianPoint.Y += (float)(cosLat * Math.Sin(lonRad));
            cartesianPoint.Z += (float)Math.Sin(latRad);
        }

        cartesianPoint /= points.Count;

        var avg = new Coordinates(
            cartesianPoint.X,
            cartesianPoint.Y,
            cartesianPoint.Z
        );

        return avg;
    }
}