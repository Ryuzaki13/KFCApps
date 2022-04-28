using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KFCApp.AppPages
{
	public static class Resouces
	{
		private static Authorization authorization;
		private static RegistrationSuperUser registrationSuperUser;
		private static SuperUser.Main superUserMain;

		public static Authorization GetAuthorization()
		{
			if (authorization == null)
			{
				authorization = new Authorization();
			}

			return authorization;
		}

		public static RegistrationSuperUser GetRegistrationSuperUser()
		{
			if (registrationSuperUser == null)
			{
				registrationSuperUser = new RegistrationSuperUser();
			}

			return registrationSuperUser;
		}

		public static SuperUser.Main GetSuperUserMain()
		{
			if (superUserMain == null)
			{
				superUserMain = new SuperUser.Main();
			}

			return superUserMain;
		}

	}
}
