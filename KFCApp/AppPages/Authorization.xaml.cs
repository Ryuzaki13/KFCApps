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
        private string code = "";
        private double blockTime = 0;
        private int blockInterval = 60000;
        private int failCounter = 0;
        private const string filename = "data.lock";

        public Authorization()
        {
            InitializeComponent();

            string chars = "QWERTYUIOPASDFGHJKLZXCVBNM0123456789@#$%&";
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
            try
            {
                //var file = File.Open(filename, FileMode.OpenOrCreate);
                //BinaryWriter binaryWriter = new BinaryWriter(file);

                Lib.Connector.GetModel().Blocking.Add(new AppData.Blocking() { BlockTime = DateTime.Now.AddMinutes(1) });
                Lib.Connector.GetModel().SaveChanges();

                //binaryWriter.Write(blockTime);
                //file.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private bool CheckBlocking()
        {
            if (File.Exists(filename) == true)
            {
                try
                {
                    //var file = File.Open(filename, FileMode.Open);
                    //BinaryReader binaryReader = new BinaryReader(file);
                    //blockTime = binaryReader.ReadDouble();
                    //file.Close();

                    var lastBlockTime = Lib.Connector.GetModel().Blocking.OrderByDescending(b => b.BlockTime).FirstOrDefault();
                    if (lastBlockTime == null)
                    {
                        return false;
                    }

                    blockTime = new TimeSpan(lastBlockTime.BlockTime.Ticks).TotalMilliseconds;

                    TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks);

                    if (blockTime > timeSpan.TotalMilliseconds)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return true;
                }
            }
            else
            {
                WriteBlocking();
                return true;
            }

            return false;
        }

        private void OnLogin(object sender, RoutedEventArgs e)
        {
            if (failCounter >= 3)
            {
                WriteBlocking();
                MessageBox.Show("Вход заблокирован на 1 минуту");
                failCounter = 0;
                return;
            }

            if (CheckBlocking() == true)
            {
                TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks);
                int time = (int)((blockTime - timeSpan.TotalMilliseconds) / 1000);
                MessageBox.Show("Вход заблокирован ещё " + time + " секунд");
                return;
            }

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
