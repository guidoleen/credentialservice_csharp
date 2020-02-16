
using System.Web;
using System.Threading.Tasks;
using System;

namespace CredentialService
{
	// REMARKS >> GetPrevious UserAgent from eg. Redis/Db >> eg. based on sessionId

	public static class LoginManager
	{
		private static string _sessionId;
		private static ICacheControl<ClientInformation> _cacheController = CacheControlService.GetCacheController();
		private static ITwoFactorAuthenticator _twofactorauthenticator = TwoFactorAuthenticatorService.GetTwoFactorAuthenticator();

		/// <summary>
		/// Login gets username and password and creates a cache for storing clieninformation
		/// </summary>
		/// <param name="username"></param>
		/// <param name="pwd"></param>
		/// <returns></returns>
		public static async Task<bool> Login(string username, string pwd)
		{
			// For bruteforce attacks
			if (!CredentialServiceHelper.IsAcceptablePwd(pwd)) return false;
			if (IsExeedMaxLoginAttempts(username)) return false;

			// Set ClientInfo into cache when valid user
			if (IsValidUser(username, pwd))
			{
				// First remove tempory item from cache
				_cacheController.RemoveItem(username);

				CookieManager.GetSessIdCookie();
				// Set session id
				SessionControl.SetSessionId(); // Set Session Id
				_sessionId = SessionControl.GetSessionId();
				_sessionId = CookieManager.CreateHashFromSessionCookie(_sessionId); // Hash Id and place in HttpOnly Cookie

				// Set HttpOnly cookie
				CookieManager.SetHttpOnlyCookie(_sessionId);

				// Set clientInformation into cache or redis or db
				// Two factor Authentication
				// Code from code service and is used for sending to client and saved in cache
				SetClientInfoIntoCache(_sessionId, null, _twofactorauthenticator.SetCode(_sessionId));

				return true;
			}
			else
			{
				// BruteForce attack >> can also be done by captcha
				await SetClientLoginAndDelayAfterLoginAttempt(username);
			}
			return false;
		}

		public static async Task<bool> TwoFactorAutenticationLogin(string inputCode)
		{
			var sessionId = CookieManager.CreateHashFromSessionCookie(CookieManager.GetSessIdCookie());
			bool result = await Task.Run(() =>
			{
				var savedCode = _cacheController.GetItem(sessionId).ClientAuthenticationCode;

				if (inputCode.Equals(savedCode)) return true;
				else return false;
			});
			return result;
		}

		private static async Task SetClientLoginAndDelayAfterLoginAttempt(string usrname)
		{
			SetClientLoginAttempt(usrname);
			await Task.Delay(1000);
		}

		private static bool IsValidUser(string usrname, string pwd)
		{
			return usrname.Equals("bla") && pwd.Equals("bla");
		}

		private static void SetClientInfoIntoCache(string sessionId, ClientInformation clientInformation, string authenticationCode)
		{
			if (clientInformation == null)
			{
				clientInformation = new ClientInformation()
				{
					ClientBrowser = GetClientBrowserInfo(),
					ClientHost = GetClientHostInfo(),
					ClientIp = GetClientIp(),
					ClientAuthenticationCode = authenticationCode
				};
			}

			_cacheController.AddItem(sessionId, clientInformation);
		}

		/// <summary>
		/// After Valid login check if client is valid based on cache information
		/// </summary>
		/// <param name="sessionId"></param>
		/// <returns></returns>
		public static bool IsValidUserAgentHostAndIp(string sessionId)
		{
			var clientBrowser = GetClientBrowserInfo();
			var clientHost = GetClientHostInfo();
			var clientIp = GetClientIp();

			var clientInformation = GetClientInformationFromCache(sessionId);

			return clientInformation.ClientBrowser.Equals(clientBrowser) && clientInformation.ClientHost.Equals(clientHost) && clientInformation.ClientIp.Equals(clientIp);
		}

		private static ClientInformation GetClientInformationFromCache(string sessionId)
		{
			return (ClientInformation)_cacheController.GetItem(sessionId) ?? new ClientInformation() { ClientBrowser = "", ClientHost = "", ClientIp = "", ClientLoginAttempt = 0 };
		}

		public static void LogOut(string sessionId)
		{
			if (_cacheController.GetItem(sessionId) != null) _cacheController.RemoveItem(sessionId);
			SessionControl.DeleteSessionId();
		}

		private static string GetClientBrowserInfo()
		{
			var clientBrowserInfo = HttpContext.Current.Request.Browser;
			return $"{clientBrowserInfo.Browser}:{clientBrowserInfo.Version}:{clientBrowserInfo.Id}";
		}

		private static string GetClientHostInfo()
		{
			var clientHostInfo = System.Net.Dns.GetHostEntry(HttpContext.Current.Request.ServerVariables["REMOTE_HOST"]).HostName;
			return $"{clientHostInfo}";
		}

		private static string GetClientIp()
		{
			var hostName = System.Net.Dns.GetHostName();
			var clientIp = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();

			return $"{clientIp}";
		}

		private static int SetClientLoginAttempt(string usrname)
		{
			if ((ClientInformation)_cacheController.GetItem(usrname) == null)
			{
				SetClientInfoIntoCache(usrname, null, "");
			}
			var clientIformation = (ClientInformation)_cacheController.GetItem(usrname);
			clientIformation.ClientLoginAttempt = clientIformation.ClientLoginAttempt + 1;

			return clientIformation.ClientLoginAttempt;
		}

		private static bool IsExeedMaxLoginAttempts(string usrname)
		{
			try
			{
				if(_cacheController.GetItem(usrname).ClientLoginAttempt > AppConfigurationSettings.MaxLoginAttempts)
					return	true;
			}
			catch
			{
				return false;
			}
	
			return false;
		}
	}

	public sealed class ClientInformation
	{
		public string ClientBrowser { get; set; }
		public string ClientHost { get; set; }
		public string ClientIp { get; set; }
		public int ClientLoginAttempt { get; set; }
		public string ClientAuthenticationCode { get; set; }
	}
}

// https://jasonwatmore.com/post/2015/06/03/csharp-incremental-delay-to-prevent-brute-force-or-dictionary-attack