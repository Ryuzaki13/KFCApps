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

namespace KFCApp
{
	public partial class AppWindow : Window
	{
		public AppWindow()
		{
			InitializeComponent();
			CheckSuperUser();
		}

		private void CheckSuperUser()
		{
			int count = Lib.Connector.GetModel().Employees.Count();
			if (count == 0)
			{
				// SuperUser не был создан
				AppFrame.Navigate(AppPages.Resouces.GetRegistrationSuperUser());
			}
			else
			{
				AppFrame.Navigate(AppPages.Resouces.GetAuthorization());
			}
		}
	}
}
