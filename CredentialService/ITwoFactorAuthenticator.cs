using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredentialService
{
	public interface ITwoFactorAuthenticator
	{
		string SetCode(string username);
		bool IsValidUser(string username);
	}
}
