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
        // Список всех сотрудников из базы
        // для отображения их в ListBox'е
        private List<AppData.Employees> Employees { get; set; }
        // Список ролей, для ComboBox'а, для выбора роли
        // сотруднику
        public List<AppData.Roles> Roles { get; set; }

        public Main()
        {
            InitializeComponent();

            // Сохранить Модель базы в переменную
            // чтобы проще было к ней обращаться
            Connection = Lib.Connector.GetModel();

            // Получить все роли из базы и
            // сохранить в переменной
            Roles = Connection.Roles.ToList();
            // Этой же переменной заполнить ListBox
            EmployeeRole.ItemsSource = Roles;

            // Это можно проигнорировать :)
            Lib.Placeholder.SetElement(EmployeePhone, "EmployeePhone", "Телефон");
            Lib.Placeholder.SetElement(EmployeePassword, "EmployeePassword", "Пароль");
            Lib.Placeholder.SetElement(EmployeeFirstName, "EmployeeFirstName", "Имя");
            Lib.Placeholder.SetElement(EmployeeLastName, "EmployeeLastName", "Фамилия");
            Lib.Placeholder.SetElement(EmployeePatronymic, "EmployeePatronymic", "Отчество");

            // Чтобы Binding работал, нужно это написать
            DataContext = this;
        }

        // Событие возникающее при нажатии на вкладку Сотрудники
        private void Activate(object sender, MouseButtonEventArgs e)
        {
            // Загрузить список сотрудников
            UpdateEmployeeList();
        }

        // Загрузить список сотрудников
        private void UpdateEmployeeList()
        {
            // Получить из базы весь список сотрудников
            Employees = Connection.Employees
                // отсортированный по ФИО
                .OrderBy(emp => new { emp.LastName, emp.FirstName, emp.Patronymic })
                .ToList();

            // Заполнить ListBox этим списком
            EmployeeList.ItemsSource = Employees;
        }

        // Событие нажатия на кнопку "Регистрация" нового сотрудника
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
            Update();

            MessageBox.Show("Данные сотрудника успешно обновлены");
        }

        private void Registration()
        {
            // Создать нового сотрудника
            var employee = new AppData.Employees();

            // Заполнить все поля
            employee.FirstName = EmployeeFirstName.Text.Trim();
            employee.LastName = EmployeeLastName.Text.Trim();
            employee.Patronymic = EmployeePatronymic.Text.Trim();
            employee.Phone = EmployeePhone.Text.Trim();
            employee.Password = EmployeePassword.Text.Trim();
            employee.Role = (EmployeeRole.SelectedItem as AppData.Roles).Name;
            
            // Добавить созданного сотрудника в базу
            Connection.Employees.Add(employee);
        }

        private void Update()
        {
            Connection.SaveChanges();
        }

        private void ClearTextBox()
        {
            EmployeeFirstName.Text = "";
            EmployeeLastName.Text = "";
            EmployeePatronymic.Text = "";
            EmployeePhone.Text = "";
            EmployeePassword.Text = "";
            EmployeeRole.SelectedIndex = -1;
        }

        private void UpdateBinding()
        {
            EmployeePhone.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            EmployeePassword.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            EmployeeFirstName.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            EmployeeLastName.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            EmployeePatronymic.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
            EmployeeRole.GetBindingExpression(ComboBox.SelectedItemProperty).UpdateTarget();
        }

        // Событие выбора сотрудника из ListBox'а
        private void EmployeeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Если был выбран сотрудник
            if (EmployeeList.SelectedIndex != -1)
            {
                // тогда с кнопки снять событие, ведущее к регистрации
                ButtonEmployeeAction.Click -= RegisterUser;
                // и повешать событие на обновление данных сотрудника
                ButtonEmployeeAction.Click += UpdateUser;
                // и обновить надпись на кнопке
                ButtonEmployeeAction.Content = "Обновить";

                // Сохранить в переменную выбранного сотрудника
                // Эта переменная самая важная!
                SelectedEmployee = EmployeeList.SelectedItem as AppData.Employees;

                // Обновить привязку текстовых полей к переменной SelectedEmployee
                UpdateBinding();
            }
            else
            {
                // Если сотрудник не был выбран

                // снять событие обновления
                ButtonEmployeeAction.Click -= UpdateUser;
                // и повешать событие регистрации
                ButtonEmployeeAction.Click += RegisterUser;
                // и поменять надпись на кнопке
                ButtonEmployeeAction.Content = "Зарегистрировать";
            }
        }
    }
}
