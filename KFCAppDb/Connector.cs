namespace KFCAppDb
{
	public class Connector
    {
		private AppModelContainer connection;

		public Connector()
		{
			connection = new AppModelContainer();
		}

		public AppModelContainer Get()
		{
			return connection;
		}
    }
}
