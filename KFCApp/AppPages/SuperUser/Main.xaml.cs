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
        public AppData.Employees SelectedEmployee { get; set; }
        private List<AppData.Employees> Employees { get; set; }
        public List<AppData.Roles> Roles { get; set; }

        public Main()
        {
            InitializeComponent();

            Roles = Lib.Connector.GetModel().Roles.ToList();
            EmployeeRole.ItemsSource = Roles;

            Lib.Placeholder.SetElement(EmployeePhone, "EmployeePhone", "Телефон");
            Lib.Placeholder.SetElement(EmployeePassword, "EmployeePassword", "Пароль");
            Lib.Placeholder.SetElement(EmployeeFirstName, "EmployeeFirstName", "Имя");
            Lib.Placeholder.SetElement(EmployeeLastName, "EmployeeLastName", "Фамилия");
            Lib.Placeholder.SetElement(EmployeePatronymic, "EmployeePatronymic", "Отчество");

            DataContext = this;
        }

        private void Activate(object sender, MouseButtonEventArgs e)
        {
            UpdateEmployeeList();
        }

        private void UpdateEmployeeList()
        {
            Employees = Lib.Connector.GetModel().Employees
                .OrderBy(emp => new { emp.LastName, emp.FirstName, emp.Patronymic })
                .ToList();

            EmployeeList.ItemsSource = Employees;
        }

        private void RegisterUser(object sender, RoutedEventArgs e)
        {
            Registration();
            Update();
            UpdateEmployeeList();
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
            var employee = new AppData.Employees();
            employee.FirstName = EmployeeFirstName.Text.Trim();
            employee.LastName = EmployeeLastName.Text.Trim();
            employee.Patronymic = EmployeePatronymic.Text.Trim();
            employee.Phone = EmployeePhone.Text.Trim();
            employee.Password = EmployeePassword.Text.Trim();
            employee.Role = (EmployeeRole.SelectedItem as AppData.Roles).Name;
            Lib.Connector.GetModel().Employees.Add(employee);
        }

        private void Update()
        {
            Lib.Connector.GetModel().SaveChanges();
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

        private void EmployeeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EmployeeList.SelectedIndex != -1)
            {
                ButtonEmployeeAction.Click -= RegisterUser;
                ButtonEmployeeAction.Click += UpdateUser;
                ButtonEmployeeAction.Content = "Обновить";

                SelectedEmployee = EmployeeList.SelectedItem as AppData.Employees;
                UpdateBinding();
            }
            else
            {
                ButtonEmployeeAction.Click -= UpdateUser;
                ButtonEmployeeAction.Click += RegisterUser;
                ButtonEmployeeAction.Content = "Зарегистрировать";
            }
        }
    }
}
