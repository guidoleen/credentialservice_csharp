using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredentialService
{
	public static class TwoFactorAuthenticatorService
	{
		public static ITwoFactorAuthenticator GetTwoFactorAuthenticator()
		{
			ITwoFactorAuthenticator authenticator = (EmailCodeAuthenticator)new EmailCodeAuthenticator();
			return authenticator;
		}
	}
}
