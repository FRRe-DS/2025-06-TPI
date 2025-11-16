

namespace ApiDePapas.Application.Tests
{
    public class DistanceServiceInternalTests
    {
        private readonly Mock<ILocalityRepository> _mock_locality_repository;

        private readonly DistanceServiceInternal _service;

        public DistanceServiceInternalTests()
        {
            _mock_locality_repository = new Mock<ILocalityRepository>();

            _service = new DistanceServiceInternal(
                _mock_locality_repository.Object
            );
        }

        [Fact]
        public void DistanceServiceInternal_ResistenciaToGoya_ReturnsCorrectDistance()
        {
            const string originCpa = "H3500";
            const string destinationCpa = "W3450";
            const double tolerance = 1.33;

                _mock_locality_repository
                    .Setup(r => r.GetByPostalCodeAsync("H3500"))
                    .ReturnsAsync(new List<Locality>
                    {
                        new Locality { postal_code = "H3500", lat = -27.4500f, lon = -58.9833f }
                    });

                _mock_locality_repository
                    .Setup(r => r.GetByPostalCodeAsync("W3450"))
                    .ReturnsAsync(new List<Locality>
                    {
                        new Locality { postal_code = "W3450", lat = -29.1333f, lon = -59.2667f }
                    });

            double distance_km = _service.GetDistanceKm(originCpa, destinationCpa).Result;

            Assert.InRange(distance_km, distance_km * (1.0 / tolerance), distance_km * tolerance);
        }
    }
}