// Models/Product.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace NutritionApi.Models
{
    public class Product
    {
        [Key]
        public string Barcode { get; set; } = default!;
        public string? Name { get; set; }
        public string? Brand { get; set; }
        public double? Calories { get; set; }       // kcal/100g (quando disponível)
        public double? Proteins { get; set; }       // g/100g
        public double? Carbohydrates { get; set; }  // g/100g
        public double? Fats { get; set; }           // g/100g
        public string? RawJson { get; set; }        // guarda JSON bruto (para audit/upgrade)
        public DateTime RetrievedAt { get; set; }
    }
}
