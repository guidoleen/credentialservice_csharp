using System;
using System.Web;
using System.Net.Mail;

namespace CredentialService
{
	public static class EmailSendReceive
	{
		/// <summary>
		/// Use Mailhog from a docker image instance (container) >> docker run --name mailhog -p 25:1025 -p 8025:8025 -d mailhog/mailhog
		/// </summary>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="subject"></param>
		/// <param name="body"></param>
		public static void SmtpSend(string from, string to, string subject, string body)
		{
			System.Net.Mail.SmtpClient smtpClient = new SmtpClient();
			smtpClient.Host = "localhost";
			smtpClient.Port = 25;

			MailMessage message = new MailMessage(from, to, subject, body);
			message.IsBodyHtml = true;
			smtpClient.Send(message);
		}
	}
}
