using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredentialService
{
	public static class CredentialServiceHelper
	{
		public static bool IsAcceptablePwd(string pwd)
		{
			if (pwd.Length < AppConfigurationSettings.MinimalPwdLength)
				return false;

			return true;
		}
	}
}
