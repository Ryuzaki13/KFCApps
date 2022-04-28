using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCApp.Lib
{
	public static class Connector
	{
		private static AppData.Employees profile;
		private static AppData.AppConnector AppDbConnector;

		public static AppData.AppConnector GetModel()
		{
			if (AppDbConnector == null)
			{
				AppDbConnector = new AppData.AppConnector();
			}

			return AppDbConnector;
		}

		public static AppData.Employees GetMyProfile()
		{
			return profile;
		}

		public static bool Authorize(string login, string password)
		{
			string phone = login.Trim();
			string pwd = password.Trim();
			profile = GetModel().Employees.Where(emp => emp.Phone == phone && emp.Password == pwd).FirstOrDefault();
			return profile != null;
		}
	}
}
