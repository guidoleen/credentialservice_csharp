using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CredentialService
{
	public static class AppConfigurationSettings
	{
		public static int ExpireSessionCookieMinutes = 15;
		public static string SessionIdName = "sessId";
		public static int MinimalPwdLength = 3;
		public static double CacheExpiration = 15; // Minutes
		public static string WrongIpAddressText = "WRONG_IP";
		public static int MaxLoginAttempts = 4;
		public static int MinTwoFactorAuthCodeLength = 100000;
		public static int MaxTwoFactorAuthCodeLength = 999999;
		public static string FromAddress = "guido@guido.nl";
	}
}