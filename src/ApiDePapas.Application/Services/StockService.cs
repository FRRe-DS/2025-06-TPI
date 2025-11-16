using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using ApiDePapas.Application.DTOs;
using ApiDePapas.Application.Interfaces;
using ApiDePapas.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace ApiDePapas.Application.Services
{
    public class StockService : IStockService
    {
        private readonly HttpClient _httpClient;
        private readonly string _externalApiUrl;

        public StockService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _externalApiUrl = configuration["ExternalApi:StockUrl"];
        }

        public async Task<ProductDetail> GetProductDetailAsync(ProductQty product)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_externalApiUrl}/products/{product.id}");
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var productDetailResponse = JsonSerializer.Deserialize<ProductDetailResponse>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var detail = new ProductDetail
                {
                    id = productDetailResponse.Id,
                    weight = productDetailResponse.Weight,
                    length = productDetailResponse.Length,
                    width = productDetailResponse.Width,
                    height = productDetailResponse.Height,
                };

                return detail;
            }
            catch (Exception ex)
            {
                // Log exception here
                throw new Exception($"Error fetching product detail for id {product.id}", ex);
            }
        }
    }
}
