using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KFCApp.AppPages.SuperUser
{
    public partial class Main : Page
    {
        // Подключение к базе данных
        private AppData.AppConnector Connection;
        // Свойство, в котором хранится ссылка на
        // выбранного сотрудника из ListBox'а, которого
        // нужно отредактировать
        public AppData.Employees SelectedEmployee { get; set; }

        public AppData.Ingredient SelectedIngredient { get; set; }

        public AppData.Dish SelectedDish { get; set; }

        // Список всех сотрудников из базы
        // для отображения их в ListBox'е
        private List<AppData.Employees> Employees { get; set; }
        // Список ролей, для ComboBox'а, для выбора роли
        // сотруднику
        public List<AppData.Roles> Roles { get; set; }

        public List<AppData.Ingredient> Ingredients { get; set; }
        public List<AppData.IngredientCategory> IngredientCategories { get; set; }
        public List<AppData.DishCategory> DishCategories { get; set; }

        public List<AppData.Ingredient> IngredientsOfDish { set; get; }
        private HashSet<AppData.Ingredient> IngredientsHashSet { set; get; }

        public Main()
        {
            InitializeComponent();

            // Сохранить Модель базы в переменную
            // чтобы проще было к ней обращаться
            Connection = Lib.Connector.GetModel();

            // Получить все роли из базы и
            // сохранить в переменной
            Roles = Connection.Roles.ToList();

            UpdateCategoryBinding();

            IngredientsHashSet = new HashSet<AppData.Ingredient>();

            // Это можно проигнорировать :)
            Lib.Placeholder.SetElement(EmployeePhone, "EmployeePhone", "Телефон (10 цифр)");
            Lib.Placeholder.SetElement(EmployeePassword, "EmployeePassword", "Пароль (от 6 символов)");
            Lib.Placeholder.SetElement(EmployeeFirstName, "EmployeeFirstName", "Имя");
            Lib.Placeholder.SetElement(EmployeeLastName, "EmployeeLastName", "Фамилия");
            Lib.Placeholder.SetElement(EmployeePatronymic, "EmployeePatronymic", "Отчество");

            // CATEGORIES

            Lib.Placeholder.SetElement(IngredientCategoryName, "IngredientCategoryName", "Имя категории");
            Lib.Placeholder.SetElement(DishCategoryName, "DishCategoryName", "Имя категории");

            // DISHES

            Lib.Placeholder.SetElement(DishName, "DishName", "Название блюда");
            Lib.Placeholder.SetElement(DishPrice, "DishPrice", "Цена");

            // Чтобы Binding работал, нужно это написать (Не надо)
            DataContext = this;

            UpdateEmployeeList();
            UpdateIngredientList();
        }

        // Событие возникающее при нажатии на вкладку Сотрудники
        private void Activate(object sender, MouseButtonEventArgs e)
        {
            // Загрузить список сотрудников
            
        }

        private void UpdateEmployeeList()
        {
            // Получить из базы весь список сотрудников
            Employees = Connection.Employees
                // отсортированный по ФИО
                .OrderBy(emp => new { emp.LastName, emp.FirstName, emp.Patronymic })
                .ToList();
            EmployeeList.ItemsSource = Employees;
            //EmployeeList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
        }

        private void UpdateIngredientList()
        {
            Ingredients = Connection.Ingredient.OrderBy(i => i.Name).ToList();
            IngredientList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
            DishIngredientList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
        }

        private void RegisterUser(object sender, RoutedEventArgs e)
        {
            // Добавление нового сотрудника
            Registration();
            // Сохранение изменений в базе данных
            Update();
            // Обновить ListBox с новым списком сотрудников
            UpdateEmployeeList();
            // Очистить текстовые поля
            ClearTextBox();

            MessageBox.Show("Сотрудник успешно зарегистрирован");
        }

        private void UpdateUser(object sender, RoutedEventArgs e)
        {
            if (EmployeeRole.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать роль");
                return;
            }

            Update();

            MessageBox.Show("Данные сотрудника успешно обновлены");
        }

        private void EmployeeReset(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Registration()
        {
            if (EmployeeRole.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать роль");
                return;
            }

            // Создать нового сотрудника
            var employee = new AppData.Employees();

            // Заполнить все поля
            employee.FirstName = EmployeeFirstName.Text.Trim();
            employee.LastName = EmployeeLastName.Text.Trim();
            employee.Patronymic = EmployeePatronymic.Text.Trim();
            employee.Phone = EmployeePhone.Text.Trim();
            employee.Password = EmployeePassword.Text.Trim();

            if (employee.FirstName.Length < 2 ||
                employee.LastName.Length < 2 ||
                employee.Patronymic.Length < 2 ||
                employee.Phone.Length < 10 ||
                employee.Password.Length < 6)
            {
                MessageBox.Show("Не все поля заполнены или не соответствуют ограничениям");
                return;
            }

            employee.Role = (EmployeeRole.SelectedItem as AppData.Roles).Name;

            // Добавить созданного сотрудника в базу
            Connection.Employees.Add(employee);
        }

        private void Update()
        {
            Connection.SaveChanges();
        }

        private void Reset()
        {
            ButtonEmployeeInsert.IsEnabled = true;
            ButtonEmployeeUpdate.IsEnabled = false;

            EmployeeList.SelectedIndex = -1;
            ClearTextBox();

            SelectedEmployee = null;
            UpdateEmployeeBinding(); 
        }

        private void ClearTextBox()
        {
            EmployeeFirstName.Text = "";
            EmployeeLastName.Text = "";
            EmployeePatronymic.Text = "";
            EmployeePhone.Text = "";
            EmployeePassword.Text = "";
        }

        private void ClearIngredientTextBox()
        {
            IngredientName.Text = "";
            IngredientCalory.Text = "";
            IngredientActive.IsChecked = true;
            IngredientCategory.SelectedIndex = -1;
        }

        private void UpdateEmployeeBinding()
        {
            EmployeePhone.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EmployeePassword.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EmployeeFirstName.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EmployeeLastName.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EmployeePatronymic.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            EmployeeRole.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateTarget();
        }

        private void UpdateIngredientBinding()
        {
            IngredientName.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            IngredientCalory.GetBindingExpression(TextBox.TextProperty)?.UpdateTarget();
            IngredientActive.GetBindingExpression(CheckBox.IsCheckedProperty)?.UpdateTarget();
            IngredientCategory.GetBindingExpression(ComboBox.SelectedItemProperty)?.UpdateTarget();
        }

        private void EmployeeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если был выбран сотрудник
            if (EmployeeList.SelectedIndex != -1)
            {
                ButtonEmployeeInsert.IsEnabled = false;
                ButtonEmployeeUpdate.IsEnabled = true;

                // Сохранить в переменную выбранного сотрудника
                // Эта переменная самая важная!
                SelectedEmployee = EmployeeList.SelectedItem as AppData.Employees;

                // Обновить привязку текстовых полей к переменной SelectedEmployee
                UpdateEmployeeBinding();

                // https://github.com/Ryuzaki13/KFCApps
            }
            else
            {
                // Если сотрудник не был выбран
                Reset();
            }
        }

        // CATEGORIES

        private void UpdateCategoryBinding()
        {
            IngredientCategories = Connection.IngredientCategory.OrderBy(i => i.Name).ToList();
            DishCategories = Connection.DishCategory.OrderBy(i => i.Name).ToList();

            DishCategoryBox.GetBindingExpression(ComboBox.ItemsSourceProperty)?.UpdateTarget();
            DishCategoryList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
            IngredientCategoryList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
        }

        private void AddIngredientCategory(object sender, RoutedEventArgs e)
        {
            string categoryName = IngredientCategoryName.Text.Trim();
            if (categoryName.Length == 0)
            {
                MessageBox.Show("Необхождимо ввести название категории");
                return;
            }

            var exists = Connection.IngredientCategory.Where(c => c.Name == categoryName).FirstOrDefault();
            if (exists != null)
            {
                MessageBox.Show("Категория с указаным названием уже существует");
                return;
            }

            IngredientCategoryName.Text = "";

            AppData.IngredientCategory category = new AppData.IngredientCategory();
            category.Name = categoryName;

            Connection.IngredientCategory.Add(category);

            Update();
            UpdateCategoryBinding();
        }

        private void AddDishCategory(object sender, RoutedEventArgs e)
        {
            string categoryName = DishCategoryName.Text.Trim();
            if (categoryName.Length == 0)
            {
                MessageBox.Show("Необхождимо ввести название категории");
                return;
            }

            var exists = Connection.DishCategory.Where(c => c.Name == categoryName).FirstOrDefault();
            if (exists != null)
            {
                MessageBox.Show("Категория с указаным названием уже существует");
                return;
            }

            DishCategoryName.Text = "";

            AppData.DishCategory category = new AppData.DishCategory();
            category.Name = categoryName;

            Connection.DishCategory.Add(category);

            Update();
            UpdateCategoryBinding();
        }

        // INGREDIENTS

        private void IngredientInsert(object sender, RoutedEventArgs e)
        {
            AppData.Ingredient ingredient = new AppData.Ingredient();
            ingredient.Name = IngredientName.Text.Trim();

            if (int.TryParse(IngredientCalory.Text.Trim(), out int calory) == true)
            {
                ingredient.Calory = calory;
            }
            else
            {
                MessageBox.Show("Некорректно заданы калории");
                return;
            }

            ingredient.Active = IngredientActive.IsChecked.Value;
            ingredient.Category = (IngredientCategory.SelectedItem as AppData.IngredientCategory).Name;

            Connection.Ingredient.Add(ingredient);
            Update();
            ClearIngredientTextBox();
            UpdateIngredientList();
        }

        private void IngredientUpdate(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void IngredientReset(object sender, RoutedEventArgs e)
        {
            ButtonIngredientInsert.IsEnabled = true;
            ButtonIngredientUpdate.IsEnabled = false;

            SelectedIngredient = null;
            UpdateIngredientBinding();
            ClearIngredientTextBox();            
        }

        private void IngredientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IngredientList.SelectedIndex == -1)
            {
                ButtonIngredientInsert.IsEnabled = true;
                ButtonIngredientUpdate.IsEnabled = false;
                ClearIngredientTextBox();
                return;
            }

            SelectedIngredient = IngredientList.SelectedItem as AppData.Ingredient;
            if (SelectedIngredient != null)
            {
                UpdateIngredientBinding();
                ButtonIngredientInsert.IsEnabled = false;
                ButtonIngredientUpdate.IsEnabled = true;
            }
        }

        // DISHES

        private void DishIngredientList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedDishIngredient = DishIngredientList.SelectedItem as AppData.Ingredient;

            if (SelectedDishIngredient != null)
            {
                IngredientsHashSet.Add(SelectedDishIngredient);

                IngredientsOfDish = IngredientsHashSet.ToList();

                IngredientsOfDishList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
            }

            DishIngredientList.SelectedIndex = -1;
        }

        private void IngredientsOfDishList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var SelectedDishIngredient = IngredientsOfDishList.SelectedItem as AppData.Ingredient;

            if (SelectedDishIngredient != null)
            {
                IngredientsHashSet.Remove(SelectedDishIngredient);

                IngredientsOfDish = IngredientsHashSet.ToList();

                IngredientsOfDishList.GetBindingExpression(ListBox.ItemsSourceProperty)?.UpdateTarget();
            }

            IngredientsOfDishList.SelectedIndex = -1;
        }

        private void ClearDish()
        {
            DishName.Text = "";
            DishPrice.Text = "";
            DishActive.IsChecked = true;
            DishCategoryBox.SelectedItem = null;
        }

        private void AddDish(object sender, RoutedEventArgs e)
        {
            if (DishCategoryBox.SelectedIndex == -1)
            {
                MessageBox.Show("Необходимо выбрать категорию");
                return;
            }

            string dishName = DishName.Text.Trim();
            decimal price = 0;
            if (decimal.TryParse(DishPrice.Text.Trim(), out price) == false)
            {
                MessageBox.Show("Некорректно введена цена");
                return;
            }

            AppData.Dish dish = new AppData.Dish();
            dish.Name = dishName;
            dish.Price = price;
            dish.DishCategory = DishCategoryBox.SelectedItem as AppData.DishCategory;
            dish.Active = DishActive.IsChecked.Value;

            Connection.Dish.Add(dish);

            Update();
            ClearDish();
        }
    }
}