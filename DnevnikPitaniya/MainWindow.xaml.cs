
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DnevnikPitaniya
{
    public partial class MainWindow : Window
    {
        private const decimal DailyCalorieNorm = 1900m;

        // Общий каталог продуктов для главного окна и каталога
        private readonly ObservableCollection<ProductInfo> products =
            new ObservableCollection<ProductInfo>();

        private readonly ObservableCollection<DiaryEntry> diaryEntries =
            new ObservableCollection<DiaryEntry>();

        public MainWindow()
        {
            InitializeComponent();

            DiaryList.ItemsSource = diaryEntries;

            // Начальный список продуктов
            products.Add(new ProductInfo("Овсяная каша", 68m, 2.4m, 1.4m, 12m));
            products.Add(new ProductInfo("Банан", 89m, 1.1m, 0.3m, 22.8m));
            products.Add(new ProductInfo("Яблоко", 52m, 0.3m, 0.2m, 14m));
            products.Add(new ProductInfo("Куриная грудка", 165m, 31m, 3.6m, 0m));
            products.Add(new ProductInfo("Рис варёный", 130m, 2.7m, 0.3m, 28m));
            products.Add(new ProductInfo("Гречка варёная", 110m, 4.2m, 1.1m, 21.3m));
            products.Add(new ProductInfo("Яйцо", 157m, 12.7m, 10.9m, 0.7m));
            products.Add(new ProductInfo("Творог", 121m, 17m, 5m, 1.8m));
            products.Add(new ProductInfo("Огурец", 15m, 0.8m, 0.1m, 3.6m));
            products.Add(new ProductInfo("Помидор", 18m, 0.9m, 0.2m, 3.9m));
            products.Add(new ProductInfo("Картофель", 77m, 2m, 0.1m, 17m));
            products.Add(new ProductInfo("Греческий йогурт", 97m, 9m, 5m, 3.6m));

            ProductCombo.ItemsSource = products;
            ProductCombo.SelectedIndex = 0;

            UpdateSummary();
        }

        // Добавление продукта в дневник
        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductInfo product = ProductCombo.SelectedItem as ProductInfo;

            if (product == null)
            {
                MessageText.Text = "Выберите продукт.";
                return;
            }

            string normalized = MassTextBox.Text.Trim().Replace(',', '.');

            decimal mass;

            bool isValidMass = decimal.TryParse(
                normalized,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out mass
            );

            if (!isValidMass || mass <= 0)
            {
                MessageText.Text = "Введите правильную массу продукта.";
                return;
            }

            ComboBoxItem selectedMeal =
                MealCombo.SelectedItem as ComboBoxItem;

            if (selectedMeal == null)
            {
                MessageText.Text = "Выберите приём пищи.";
                return;
            }

            string meal = selectedMeal.Content.ToString();

            // Расчёт питательной ценности
            decimal factor = mass / 100m;

            DiaryEntry entry = new DiaryEntry
            {
                Meal = meal,
                Product = product.Name,
                Mass = mass,
                Calories = product.Calories * factor,
                Protein = product.Protein * factor,
                Fat = product.Fat * factor,
                Carbs = product.Carbs * factor
            };

            diaryEntries.Add(entry);

            MassTextBox.Clear();
            MessageText.Text = "Продукт успешно добавлен!";

            UpdateSummary();
        }

        // Пересчёт калорий и КБЖУ
        private void UpdateSummary()
        {
            decimal calories = 0;
            decimal protein = 0;
            decimal fat = 0;
            decimal carbs = 0;

            foreach (DiaryEntry entry in diaryEntries)
            {
                calories += entry.Calories;
                protein += entry.Protein;
                fat += entry.Fat;
                carbs += entry.Carbs;
            }

            decimal remaining = DailyCalorieNorm - calories;

            EatenText.Text = calories.ToString("0") + " ккал";
            RemainingText.Text = remaining.ToString("0") + " ккал";

            ProteinText.Text =
                "Белки: " + protein.ToString("0.0") + " / 100 г";

            FatText.Text =
                "Жиры: " + fat.ToString("0.0") + " / 70 г";

            CarbsText.Text =
                "Углеводы: " + carbs.ToString("0.0") + " / 200 г";

            ProteinBar.Value = Math.Min((double)protein, 100);
            FatBar.Value = Math.Min((double)fat, 70);
            CarbsBar.Value = Math.Min((double)carbs, 200);
        }

        // Открытие каталога продуктов
        private void Products_Click(object sender, RoutedEventArgs e)
        {
            ProductsWindow window = new ProductsWindow(products);
            window.Owner = this;
            window.ShowDialog();
        }

        // Остальные пункты меню

        private void Diary_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Вы находитесь в разделе «Дневник».");
        }

        private void Weight_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел «История веса» будет создан следующим этапом.");
        }

        private void Statistics_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел «Статистика» будет создан следующим этапом.");
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Раздел «Профиль» будет создан следующим этапом.");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Дневник питания\n\n" +
                "Приложение для учёта питания и контроля КБЖУ.\n" +
                "Разработано на C# и WPF.",
                "О программе");
        }
    }

    // Информация о продукте на 100 граммов
    public class ProductInfo
    {
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

    // Запись о съеденном продукте
    public class DiaryEntry
    {
        public string Meal { get; set; }
        public string Product { get; set; }
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
}
