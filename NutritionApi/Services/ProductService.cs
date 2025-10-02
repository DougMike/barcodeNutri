using System;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NutritionApi.Data;
using NutritionApi.Models;

namespace NutritionApi.Services
{
    public class ProductService
    {
        private readonly AppDbContext _db;
        private readonly HttpClient _http;
        private readonly ILogger<ProductService> _logger;

        public ProductService(AppDbContext db, HttpClient http, ILogger<ProductService> logger)
        {
            _db = db;
            _http = http;
            _logger = logger;
        }

        public async Task<Product?> GetProductAsync(string barcode)
        {
            _db.ChangeTracker.Clear();
            // 1) tenta no cache (SQLite)
            var cached = await _db.Products.AsNoTracking().FirstOrDefaultAsync(bc => bc.Barcode == barcode);
            if (cached != null) return cached;

            // 2) consulta OpenFoodFacts
            var fetched = await FetchFromOpenFoodFacts(barcode);
            if (fetched != null)
            {
                try
                {
                    _db.Products.Add(fetched);
                    await _db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Falha ao salvar produto no cache");
                }
            }

            return fetched;
        }

        private async Task<Product?> FetchFromOpenFoodFacts(string barcode)
        {
            var url = $"https://world.openfoodfacts.org/api/v0/product/{barcode}.json";
            HttpResponseMessage res;
            try
            {
                res = await _http.GetAsync(url);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro HTTP ao acessar OpenFoodFacts");
                return null;
            }

            if (!res.IsSuccessStatusCode) return null;

            using var stream = await res.Content.ReadAsStreamAsync();
            using var doc = await JsonDocument.ParseAsync(stream);
            var root = doc.RootElement;

            if (root.TryGetProperty("status", out var statusEl) && statusEl.GetInt32() != 1)
                return null;

            if (!root.TryGetProperty("product", out var productEl))
                return null;

            string? name = productEl.TryGetProperty("product_name", out var pn) ? pn.GetString() : null;
            string? brand = productEl.TryGetProperty("brands", out var br) ? br.GetString() : null;

            double? energy = null;
            double? proteins = null;
            double? carbs = null;
            double? fats = null;

            if (productEl.TryGetProperty("nutriments", out var nutr))
            {
                energy = TryGetDouble(nutr, "energy-kcal_100g") ?? TryGetDouble(nutr, "energy_100g");
                proteins = TryGetDouble(nutr, "proteins_100g");
                carbs = TryGetDouble(nutr, "carbohydrates_100g");
                fats = TryGetDouble(nutr, "fat_100g") ?? TryGetDouble(nutr, "fats_100g");
            }

            var product = new Product
            {
                Barcode = barcode,
                Name = name,
                Brand = brand,
                Calories = energy,
                Proteins = proteins,
                Carbohydrates = carbs,
                Fats = fats,
                RawJson = productEl.GetRawText(),
                RetrievedAt = DateTime.UtcNow
            };

            return product;
        }

        private double? TryGetDouble(JsonElement el, string key)
        {
            if (!el.TryGetProperty(key, out var token)) return null;
            if (token.ValueKind == JsonValueKind.Number && token.TryGetDouble(out var d)) return d;
            if (token.ValueKind == JsonValueKind.String && double.TryParse(token.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d2)) return d2;
            return null;
        }
    }
}
