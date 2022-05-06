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
// Для записи в файл отметки времени блокировки входа
using System.IO;

namespace KFCApp.AppPages
{
    public partial class Authorization : Page
    {
        private AppData.AppConnector connection;
        private string code = "";

        private AppData.Blocking blocking;
        private int failCounter = 0;

        private const string chars = "QWERTYUIOPASDFGHJKLZXCVBNM0123456789@#$%&";

        public Authorization()
        {
            InitializeComponent();

            connection = Lib.Connector.GetModel();
                        
            Random random = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 6; i++)
            {
                int a = random.Next(0, chars.Length - 1);
                code += chars.Substring(a, 1);
            }

            Captcha.Content = code;

#if DEBUG
            Phone.Text = "9634364862";
            Password.Password = "123456";
#endif
        }

        private void WriteBlocking()
        {
            connection.Blocking.Add(new AppData.Blocking() { BlockTime = DateTime.Now.AddMinutes(1) });
            connection.SaveChanges();
        }

        private bool CheckBlocking()
        {
            blocking = Lib.Connector.GetModel().Blocking.OrderByDescending(b => b.BlockTime).FirstOrDefault();
            if (blocking == null) return false;

            return blocking.BlockTime > DateTime.Now;
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            // Проверить количество неудачных попыток входа
            if (failCounter >= 3)
            {
                // Если лимит превышен, то записать в базу время разблокировки
                WriteBlocking();
                MessageBox.Show("Вход заблокирован на 1 минуту");
                failCounter = 0;
                return;
            }

            // Проверить, было ли приложение заблокировано
            if (CheckBlocking() == true)
            {
                // Вычислить сколько секунд осталось до разблокировки
                int time = (int)(blocking.BlockTime - DateTime.Now).TotalSeconds;
                MessageBox.Show("Вход заблокирован ещё " + time + " секунд");
                return;
            }

            // Проверить введенный код проверки
            if (TextCaptcha.Text != code)
            {
                MessageBox.Show("Неверный код проверки");
                failCounter++;
                return;
            }

            if (Lib.Connector.Authorize(Phone.Text, Password.Password) == true)
            {
                var profile = Lib.Connector.GetMyProfile();
                if (profile == null)
                {
                    MessageBox.Show("Ошибка авторизации");
                    failCounter++;
                    return;
                }

                var userRole = profile.Role;

                switch (userRole)
                {
                    case "SuperUser":
                        // open SuperUser Page...
                        NavigationService.Navigate(AppPages.Resouces.GetSuperUserMain());
                        break;
                    case "Manager":
                        // open Manager Page...
                        break;

                    // etc...

                    default: return;
                }
            }
            else
            {
                MessageBox.Show("Неверный логин/пароль");
                failCounter++;
            }
        }
    }
}
