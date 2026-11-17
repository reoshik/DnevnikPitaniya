namespace DnevnikPitaniya;

// Запись о съеденном продукте
    public class DiaryEntry
    {
        public string Meal { get; set; } = "";
        public string Product { get; set; } = "";
        public decimal Mass { get; set; }
        public decimal Calories { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Carbs { get; set; }

        public string Details
        {
            get
            {
                return Mass.ToString("0") + " г · Б: " +
                       Protein.ToString("0.0") + " г · Ж: " +
                       Fat.ToString("0.0") + " г · У: " +
                       Carbs.ToString("0.0") + " г";
            }
        }

        public string CaloriesText
        {
            get
            {
                return Calories.ToString("0") + " ккал";
            }
        }
    }
