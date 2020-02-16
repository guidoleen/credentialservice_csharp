using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CredentialService
{
	public class EmailCodeAuthenticator : ITwoFactorAuthenticator
	{
		private string _userName; // eg. email address
		private string _generatedcode;
		private string GetCodeFromService()
		{
			string result = Task<string>.Run(() => { return GenerateCode().ToString(); } ).Result;
			_generatedcode = result;
			return result;
		}

		public bool IsValidUser(string userName)
		{
			return false;
		}

		public string SetCode(string userName)
		{
			var codeFromService = GetCodeFromService();
			SendCodeToEmail(userName);
			return this._generatedcode;
		}

		private int GenerateCode()
		{
			Random random = new Random();
			return random.Next(AppConfigurationSettings.MinTwoFactorAuthCodeLength, AppConfigurationSettings.MaxTwoFactorAuthCodeLength);
		}

		private void SendCodeToEmail(string userName)
		{
			EmailSendReceive.SmtpSend(AppConfigurationSettings.FromAddress, userName, "CodeForLogin", _generatedcode);
		}
	}
}
