using System;
using System.Globalization;
using System.Collections.Generic;
using System.Numerics;

using ApiDePapas.Application.Utils;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Repositories;

// Es una mejor aproximación que DistanceServiceInMemory. Sirve para
// calcular distancias entre departamentos en lugar de provincias, por lo
// que usa los 5 primeros dígitos del código postal y promedia las coordenadas
// geográficas de las localidades con ese código postal.

// Puede utilizarse una API externa en el futuro si se quiere precisión a nivel
// calle/dirección.

namespace ApiDePapas.Application.Services
{
    public class DistanceServiceInternal : IDistanceService
    {
        private readonly ILocalityRepository _locality_repository;

        public DistanceServiceInternal(
            ILocalityRepository localityRepository)
        {
            _locality_repository = localityRepository;
        }

        public async Task<float> GetDistanceKm(string originCpa, string destinationCpa)
        {
            var possibleOriginLocalities = await _locality_repository.GetByPostalCodeAsync(originCpa);
            var possibleDestinationLocalities = await _locality_repository.GetByPostalCodeAsync(destinationCpa);

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

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(Angle.DegToRad(p1.Lat)) * Math.Cos(Angle.DegToRad(p2.Lat)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return (float)(R * c);
        }

        private static Coordinates GetAverageCoordinates(List<Coordinates> points)
        {
            if (points == null || points.Count == 0)
            {
                throw new ArgumentException("La lista de puntos no puede ser nula o vacía.");
            }

            Vector3 cartesian_point = new Vector3(0.0f, 0.0f, 0.0f);

            foreach (Coordinates p in points)
            {
                double lat_rad = Angle.DegToRad(p.Lat);
                double lon_rad = Angle.DegToRad(p.Lon);

                double cos_lat = Math.Cos(lat_rad);

                cartesian_point.X += (float)(cos_lat * Math.Cos(lon_rad));
                cartesian_point.Y += (float)(cos_lat * Math.Sin(lon_rad));
                cartesian_point.Z += (float)Math.Sin(lat_rad);
            }

            cartesian_point /= points.Count;

            double hyp = Math.Sqrt(
                cartesian_point.X * cartesian_point.X +
                cartesian_point.Y * cartesian_point.Y
            );

            double lon_center = Math.Atan2(cartesian_point.Y, cartesian_point.X);
            double lat_center = Math.Atan2(cartesian_point.Z, hyp);

            Coordinates center = new Coordinates(
                (float)Angle.RadToDeg(lat_center),
                (float)Angle.RadToDeg(lon_center)
            );

            return center;
        }
    }
}