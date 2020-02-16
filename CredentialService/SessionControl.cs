
using System.Web;

namespace CredentialService
{
    public static class SessionControl
    {
        private static bool _isAuthorized = false;
        private static string _sessionId;
        
		public static void SetSessionId()
		{
			_sessionId = HttpContext.Current.Session.SessionID;
		}

        public static void AddSessionItem(string itemName, object sessionObj)
        {
            if (_sessionId != null) _sessionId = HttpContext.Current.Session.SessionID;
            HttpContext.Current.Session[itemName] = sessionObj;
        }
        public static void RemoveSessionItem(string itemName)
        {
            HttpContext.Current.Session.Remove(itemName);
        }

        public static string GetSessionId()
        {
            return _sessionId;
        }

        public static object GetSessionObject(string itemName)
        {
            return HttpContext.Current.Session[itemName];
        }
		public static void DeleteSessionId()
		{
			if(_sessionId != null)
				HttpContext.Current.Session.Remove(_sessionId);
			HttpContext.Current.Session.Abandon();
		}
    }
}