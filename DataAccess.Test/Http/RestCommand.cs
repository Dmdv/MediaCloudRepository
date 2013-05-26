namespace DataAccess.Test.Http
{
	public static class RestCommand
	{
		private const string UserManager = "/Service.svc/UserManager";
		private const string DeviceManager = "/Service.svc/MediaRepositoryService";

		public static string CreateUser
		{
			get { return UserManager + "/create/user"; }
		}

		public static string IsUserExists
		{
			get { return UserManager + "/isuserexists"; }
		}

		public static string CreateDevice
		{
			get { return DeviceManager + "/create/device"; }
		}

		public static string UploadOriginal
		{
			get { return DeviceManager + "/uploadoriginal/{filename}/{deviceid}"; }
		}

		public static string UploadPreview
		{
			get { return DeviceManager + "/uploadpreview/{filename}/{deviceid}"; }
		}
	}
}