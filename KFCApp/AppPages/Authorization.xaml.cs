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

namespace KFCApp.AppPages
{
	public partial class Authorization : Page
	{
		public Authorization()
		{
			InitializeComponent();

#if DEBUG
			Phone.Text = "9634364862";
			Password.Password = "123456";
#endif

		}

		private void OnLogin(object sender, RoutedEventArgs e)
		{
			if (Lib.Connector.Authorize(Phone.Text, Password.Password) == true)
			{
				var profile = Lib.Connector.GetMyProfile();
				if (profile == null)
                {
					MessageBox.Show("Ошибка авторизации");
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
			}
		}
	}
}
