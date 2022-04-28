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
using System.Security.Cryptography;

namespace KFCApp.AppPages
{

    public enum ROLES
    {
        Client,
        Cook,
        Cashier,
        Manager,
        SuperUser,
    };

    public partial class RegistrationSuperUser : Page
    {
        // Save pointer to database context
        private readonly AppData.AppConnector ConnectorPtr;
        private readonly string SuperUserRole = Enum.GetName(typeof(ROLES), ROLES.SuperUser);

        public RegistrationSuperUser()
        {
            InitializeComponent();

            ConnectorPtr = Lib.Connector.GetModel();

            GenerateRoles();
        }

        private void GenerateRoles()
        {
            var roles = ConnectorPtr.Roles.ToList();
            foreach (var role in roles)
            {
                ConnectorPtr.Roles.Remove(role);
            }
            ConnectorPtr.SaveChanges();

            var roleNames = Enum.GetNames(typeof(ROLES));

            foreach (var roleName in roleNames)
            {
                AppData.Roles role = new AppData.Roles();
                role.Name = roleName;
                ConnectorPtr.Roles.Add(role);
            }

            ConnectorPtr.SaveChanges();
        }

        private void RegistrationClick(object sender, RoutedEventArgs ev)
        {
            string phone = Login.Text.Trim();
            string password = Password.Password.Trim();
            string firstName = FirstName.Text.Trim();
            string lastName = LastName.Text.Trim();
            string patronymic = Patronymic.Text.Trim();
            if (phone.Length != 10)
            {
                MessageBox.Show("Длина логина должна составлять 10 символов (номер телефона)");
                return;
            }
            if (password.Length < 6 || password.Length > 20)
            {
                MessageBox.Show("Длина пароля должна лежать в диапазоне 6..20 символов");
                return;
            }
            if (firstName.Length < 2 || lastName.Length < 2 || patronymic.Length < 2)
            {
                MessageBox.Show("Необходимо заполнить ФИО");
                return;
            }

            AppData.Employees employee = new AppData.Employees();
            employee.Phone = phone;
            employee.Password = password;
            employee.Role = SuperUserRole;
            employee.FirstName = firstName;
            employee.LastName = lastName;
            employee.Patronymic = patronymic;

            ConnectorPtr.Employees.Add(employee);
            int result = ConnectorPtr.SaveChanges();
            if (result == 0)
            {
                MessageBox.Show("Регистрация не была выполнена");
                return;
            }

            NavigationService.Navigate(Resouces.GetAuthorization());
        }

    }
}
