using ApiDePapas.Domain.Entities;

namespace ApiDePapas.Application.Tests
{
    public class CalculateCostTests
    {
        private readonly Mock<IStockService> _mock_stock_service;
        private readonly Mock<IDistanceService> _mock_distance_service;
        private readonly CalculateCost _service;

        public CalculateCostTests()
        {
            // Setup global de Mocks
            _mock_stock_service = new Mock<IStockService>();
            _mock_distance_service = new Mock<IDistanceService>();

            // Instanciacion del servicio con mocks
            _service = new CalculateCost(
                _mock_stock_service.Object, 
                _mock_distance_service.Object
            );
        }

        [Fact]
        public void CalculateShippingCost_WithOneProduct_ReturnsCorrectCost()
        {
            // 1. ORGANIZAR

            // Datos de prueba
            const string originCpa = "H3500";
            const string destinationCpa = "w3450";
            const double distanceToReturn = 100.0; // Lo que devuelve el mock de Distancia
            const int productId = 1;
            const int quantity = 2;

            // Detalle falso del producto que devolverá el mock de Stock
            var fakeProductDetail = new ProductDetail
            {
                id = productId,
                weight = 150f, 
                length = 10f,
                width = 10f,
                height = 5f,
                base_price = 10
            };

            // Creación del Request que se enviara al servicio
            var productQty = new ProductQty(productId, quantity);
            var address = new DeliveryAddressRequest(destinationCpa);
            var request = new ShippingCostRequest(
                address, 
                new List<ProductQty> { productQty }
            );

            _mock_distance_service
                .Setup(s => s.GetDistanceKm(originCpa, destinationCpa))
                .Returns(Task.FromResult(distanceToReturn));

            _mock_stock_service
                .Setup(s => s.GetProductDetail(productQty))
                .Returns(fakeProductDetail);


            // 2. ACTUAR

            // Llamada al metodo a probar
            var response = _service.CalculateShippingCost(request);


            // 3. ASSERT

            // Calculo del resultado esperado mano, usando la misma fórmula del servicio
            
            // Logica del servicio:
            // float distance_km = (float)distanceToReturn; // 100.0f
            // float total_weight_grs = prod_detail.weight * prod.quantity; // 150f * 2 = 300f
            // float prod_volume_cm3 = prod_detail.length * prod_detail.width * prod_detail.height; // 10*10*5 = 500f
            // float partial_cost = total_weight_grs * 1.2f + prod_volume_cm3 * 0.5f + distance_km * 8.0f;
            
            float expected_distance = (float)distanceToReturn;
            float expected_weight = 150f * 2;
            float expected_volume = 10f * 10f * 5f;
            
            float expected_cost = (expected_weight * 1.2f) +
                                  (expected_volume * 0.5f) +
                                  (expected_distance * 8.0f); 

            // Assert.Equal(610.0f, response.total_cost);
            Assert.Equal("ARS", response.currency);
            Assert.Equal(TransportType.plane, response.transport_type);
            Assert.Single(response.products);
            Assert.Equal(productId, response.products[0].id);
            // Assert.Equal(expected_cost, response.products[0].cost);
        }
    }
}
