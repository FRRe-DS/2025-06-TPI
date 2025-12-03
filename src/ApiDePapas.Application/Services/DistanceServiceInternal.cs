using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; // Necesario para Task
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using ApiDePapas.Domain.Repositories;
using System.Text.RegularExpressions; // Para limpiar el CP

namespace ApiDePapas.Application.Services
{
    public class DistanceServiceInternal : IDistanceService
    {
        private readonly ILocalityRepository _locality_repository;

        public DistanceServiceInternal(ILocalityRepository localityRepository)
        {
            _locality_repository = localityRepository;
        }

        public (double lat, double lon) GetAverageCoordinates(List<(double, double)> points)
        {
            if (points == null || !points.Any())
            {
                // Retornar 0,0 en lugar de lanzar error para ser más resiliente
                return (0, 0);
            }

            double x = 0.0;
            double y = 0.0;
            double z = 0.0;
            int count = 0;

            foreach (var (latDeg, lonDeg) in points)
            {
                double latRad = DegreesToRadians(latDeg);
                double lonRad = DegreesToRadians(lonDeg);
                double cosLat = Math.Cos(latRad);

                x += cosLat * Math.Cos(lonRad);
                y += cosLat * Math.Sin(lonRad);
                z += Math.Sin(latRad);
                count++;
            }

            x /= count;
            y /= count;
            z /= count;

            double lonCenter = Math.Atan2(y, x);
            double hyp = Math.Sqrt(x * x + y * y);
            double latCenter = Math.Atan2(z, hyp);

            return (RadiansToDegrees(latCenter), RadiansToDegrees(lonCenter));
        }

        // Método auxiliar para limpiar el CP
        // Transforma "H3500AAA" -> "H3500" (Conserva la letra de provincia y los números)
        private static string CleanPostalCode(string cpa)
        {
            if (string.IsNullOrEmpty(cpa)) return "";

            // Regex: ^[A-Za-z] busca una letra al inicio
            //        \d+      busca los números que le siguen
            // Esto capturará "H3500" e ignorará "AAA" del final.
            var match = Regex.Match(cpa, @"^[A-Za-z]\d+");

            if (match.Success)
            {
                return match.Value.ToUpper(); // Retorna H3500 en mayúsculas
            }
            return Regex.Replace(cpa, @"\s+", "").ToUpper(); // Si no coincide, retorna el CP sin espacios en mayúsculas
        }

        public async Task<double> GetDistanceKm(string originCpa, string destinationCpa)
        {
            string cleanOrigin = CleanPostalCode(originCpa);
            string cleanDest = CleanPostalCode(destinationCpa);

            List<Locality> possibleOriginLocalities = await _locality_repository.GetByPostalCodeAsync(cleanOrigin);
            List<Locality> possibleDestinationLocalities = await _locality_repository.GetByPostalCodeAsync(cleanDest);

            if (!possibleOriginLocalities.Any() || !possibleDestinationLocalities.Any()) 
            {
                return GetDistanceByLetter(FirstLetter(originCpa), FirstLetter(destinationCpa));
            }

            List<(double lat, double lon)> possibleOriginCoords = possibleOriginLocalities
                .Select(l => ((double)l.lat, (double)l.lon))
                .ToList();

            List<(double lat, double lon)> possibleDestinationCoords = possibleDestinationLocalities
                .Select(l => ((double)l.lat, (double)l.lon))
                .ToList();

            var originCentroid = GetAverageCoordinates(possibleOriginCoords);
            var destinationCentroid = GetAverageCoordinates(possibleDestinationCoords);

            return HaversineKm(originCentroid.lat, originCentroid.lon, destinationCentroid.lat, destinationCentroid.lon);
        }

        private double GetDistanceByLetter(char originCpaLetter, char destinationCpaLetter)
        {
            char o = originCpaLetter;
            char d = destinationCpaLetter;

            bool originExists = coords.TryGetValue(o, out var O);
            bool destinationExists = coords.TryGetValue(d, out var D);

            if (!originExists || !destinationExists)
            {
                throw new ArgumentException(
                    $"Letra de CPA inválida: origen='{o}', destino='{d}'"
                );
            }

            return HaversineKm(O.lat, O.lon, D.lat, D.lon);
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            double dLat = DegreesToRadians(lat2 - lat1);
            double dLon = DegreesToRadians(lon2 - lon1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double degrees) => degrees * (double)Math.PI / 180.0f;
        private static double RadiansToDegrees(double radians) => radians * 180.0f / (double)Math.PI;

        private static char FirstLetter(string cpa)
            => string.IsNullOrWhiteSpace(cpa) ? 'H' : char.ToUpperInvariant(cpa.Trim()[0]);

        private static readonly Dictionary<char, (double lat, double lon)> coords = new()
        {
            ['A']=(-24.79,-65.41), ['B']=(-34.61,-58.38), ['C']=(-34.60,-58.38), ['D']=(-33.30,-66.34),
            ['E']=(-31.73,-60.53), ['F']=(-29.41,-66.86), ['G']=(-27.79,-64.26), ['H']=(-27.45,-58.99),
            ['J']=(-31.54,-68.52), ['K']=(-28.47,-65.79), ['L']=(-36.62,-64.29), ['M']=(-32.89,-68.83),
            ['N']=(-27.36,-55.90), ['P']=(-26.18,-58.18), ['Q']=(-38.95,-68.06), ['R']=(-41.13,-71.31),
            ['S']=(-31.63,-60.70), ['T']=(-26.83,-65.22), ['U']=(-43.30,-65.10), ['V']=(-54.80,-68.30),
            ['W']=(-27.47,-58.83), ['X']=(-31.42,-64.18), ['Y']=(-24.19,-65.30), ['Z']=(-51.62,-69.22),
        };
    }
}