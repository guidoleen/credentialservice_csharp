using System;
using System.Text;
using System.Web;

namespace CredentialService
{
	public static class CookieManager
	{
		private static string _sessioncookiename = AppConfigurationSettings.SessionIdName;
		public static void SetHttpOnlyCookie(string cookieSessionValue)
		{
			HttpCookie cookie = new HttpCookie(_sessioncookiename);
			cookie.Value = cookieSessionValue;
			cookie.HttpOnly = true;
			cookie.Secure = true;
			cookie.Expires = DateTime.Now.AddMinutes( Convert.ToDouble(AppConfigurationSettings.ExpireSessionCookieMinutes) );

			HttpContext.Current.Response.AppendCookie(cookie);
		}

		public static string GetSessIdCookie()
		{
			string result = "";
			try
			{
				result = HttpContext.Current.Request.Cookies.Get(_sessioncookiename).Value?.ToString();
			}
			catch(Exception ee)
			{
				result = "";
			}
				
			return result;
		}

		public static string CreateHashFromSessionCookie(string input)
		{
			if (input == null || input.Equals("")) return "";
			System.Security.Cryptography.SHA256 sha256;
			StringBuilder builder = new StringBuilder();

			byte[] inputBuffer = System.Text.Encoding.UTF8.GetBytes(input);
			for (int i = 0; i < inputBuffer.Length; i++)
			{
				builder.Append(inputBuffer[i].ToString("x2")); // Formats string to hexadecimal value
			}
			return builder?.ToString() ?? "";
		}
	}
}