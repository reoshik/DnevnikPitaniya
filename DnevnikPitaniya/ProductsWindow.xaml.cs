using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace DnevnikPitaniya
{
    public partial class ProductsWindow : Window
    {
        private readonly ObservableCollection<ProductInfo> products;
        private readonly ListCollectionView productsView;

        private ProductInfo? editingProduct;
        private bool isReady;

        public ProductsWindow(ObservableCollection<ProductInfo> products)
        {
            InitializeComponent();

            this.products = products;

            // поиск не меняет список продуктов в главном окне.
            productsView = new ListCollectionView(products);
            productsView.Filter = MatchesSearch;

            ProductsGrid.ItemsSource = productsView;

            isReady = true;

            ResetEditor();
            UpdateCount();
        }

        private bool MatchesSearch(object item)
        {
            if (item is not ProductInfo product)
                return false;

            string query = SearchBox.Text.Trim();

            return product.Name.Contains(
                query,
                StringComparison.CurrentCultureIgnoreCase);
        }

        private void SearchBox_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            if (!isReady)
                return;

            productsView.Refresh();
            UpdateCount();
        }

        private void ProductsGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!isReady)
                return;

            if (ProductsGrid.SelectedItem is not ProductInfo product)
            {
                ResetEditor();
                return;
            }

            editingProduct = product;

            EditorTitle.Text = "Редактирование";
            SaveButton.Content = "Сохранить изменения";
            DeleteButton.IsEnabled = true;

            NameBox.Text = product.Name;
            CaloriesBox.Text = Format(product.Calories);
            ProteinBox.Text = Format(product.Protein);
            FatBox.Text = Format(product.Fat);
            CarbsBox.Text = Format(product.Carbs);

            StatusText.Text = "";
        }

        private void NewProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductsGrid.SelectedItem = null;
            ResetEditor();
            NameBox.Focus();
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            string name = NameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name))
            {
                ShowError("Введите название продукта.");
                NameBox.Focus();
                return;
            }

            bool duplicate = products.Any(product =>
                !ReferenceEquals(product, editingProduct) &&
                string.Equals(
                    product.Name.Trim(),
                    name,
                    StringComparison.CurrentCultureIgnoreCase));

            if (duplicate)
            {
                ShowError("Продукт с таким названием уже существует.");
                return;
            }

            if (!TryReadNumber(CaloriesBox.Text, out decimal calories) ||
                !TryReadNumber(ProteinBox.Text, out decimal protein) ||
                !TryReadNumber(FatBox.Text, out decimal fat) ||
                !TryReadNumber(CarbsBox.Text, out decimal carbs))
            {
                ShowError(
                    "Заполните КБЖУ числами от нуля. " +
                    "Можно использовать запятую или точку.");
                return;
            }

            if (protein > 100 || fat > 100 || carbs > 100 ||
                protein + fat + carbs > 100)
            {
                ShowError(
                    "Сумма белков, жиров и углеводов " +
                    "не должна превышать 100 г на 100 г продукта.");
                return;
            }

            if (calories > 1000)
            {
                ShowError("Проверьте калорийность на 100 г продукта.");
                return;
            }

            bool isNew = editingProduct == null;

            var updatedProduct = new ProductInfo(
                name, calories, protein, fat, carbs);

            if (isNew)
            {
                products.Add(updatedProduct);
            }
            else
            {
                int index = products.IndexOf(editingProduct!);

                if (index < 0)
                {
                    ShowError("Продукт не найден. Выберите его заново.");
                    return;
                }

                products[index] = updatedProduct;
            }

            productsView.Refresh();
            ProductsGrid.SelectedItem = null;
            ResetEditor();
            UpdateCount();

            ShowSuccess(isNew
                ? "Продукт добавлен в каталог."
                : "Изменения сохранены.");
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductInfo? product = editingProduct;

            if (product == null)
                return;

            MessageBoxResult result = MessageBox.Show(
                $"Удалить продукт «{product.Name}»?\n\n" +
                "Уже добавленные записи дневника останутся.",
                "Удаление продукта",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            products.Remove(product);

            productsView.Refresh();
            ProductsGrid.SelectedItem = null;
            ResetEditor();
            UpdateCount();

            ShowSuccess("Продукт удалён.");
        }

        private void ResetEditor()
        {
            editingProduct = null;

            EditorTitle.Text = "Новый продукт";
            SaveButton.Content = "Добавить продукт";
            DeleteButton.IsEnabled = false;

            NameBox.Clear();
            CaloriesBox.Clear();
            ProteinBox.Clear();
            FatBox.Clear();
            CarbsBox.Clear();

            StatusText.Text = "";
        }

        private void UpdateCount()
        {
            int visible = productsView.Cast<ProductInfo>().Count();

            CountText.Text = visible == 0
                ? "Продукты не найдены"
                : $"Показано: {visible} · Всего в каталоге: {products.Count}";
        }

        private static bool TryReadNumber(
            string text,
            out decimal value)
        {
            string normalized = text.Trim().Replace(',', '.');

            return decimal.TryParse(
                       normalized,
                       NumberStyles.AllowDecimalPoint,
                       CultureInfo.InvariantCulture,
                       out value)
                   && value >= 0;
        }

        private static string Format(decimal value)
        {
            return value.ToString(CultureInfo.CurrentCulture);
        }

        private void ShowError(string message)
        {
            StatusText.Foreground =
                System.Windows.Media.Brushes.Firebrick;

            StatusText.Text = message;
        }

        private void ShowSuccess(string message)
        {
            StatusText.Foreground =
                new System.Windows.Media.SolidColorBrush(
                    System.Windows.Media.Color.FromRgb(85, 117, 44));

            StatusText.Text = message;
        }
    }
}