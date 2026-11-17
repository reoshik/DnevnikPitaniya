namespace DnevnikPitaniya;

// Информация о продукте на 100 граммов
    public class ProductInfo
    {
        public string Brand { get; set; } = "";
        public string? PhotoUrl { get; set; }
        public string Name { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Carbs { get; set; }

        public ProductInfo(
            string name,
            decimal calories,
            decimal protein,
            decimal fat,
            decimal carbs)
        {
            Name = name;
            Calories = calories;
            Protein = protein;
            Fat = fat;
            Carbs = carbs;
        }
    }
