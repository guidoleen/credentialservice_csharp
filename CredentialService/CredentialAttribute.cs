using System;
using System.Web;
using System.Web.Mvc;

namespace CredentialService
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class CredentialAttribute : AuthorizeAttribute
    {
        public bool IsValidUser = false;
        private string _userName;
        public CredentialAttribute(string userName) 
        {
			this._userName = userName;
        }
        private bool IsValidUserCheck()
        {
			var sessionId = "";
			try
			{
				sessionId = HttpContext.Current.Request.Cookies.Get(AppConfigurationSettings.SessionIdName).Value.ToString();
			}
			catch
			{
				return false;
			}

			if (!this._userName.Equals("bla"))
				return false;

			if (!LoginManager.IsValidUserAgentHostAndIp(sessionId))
				return false;

			return true;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            // Cache clearing to prevent using the cache
            this.SetCachePolicy(filterContext); // HttpContext Cache
			IsValidUser = IsValidUserCheck();

			if (!IsValidUser)
            {
                filterContext.Result = new RedirectResult($"~/Login");
            }
			else
			{
				return;
			}
            // base.OnAuthorization(filterContext);
        }
        private void RedirectToErrorPage()
        {
            ViewResult viewResult = new ViewResult();
        }

        protected void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = this.OnCacheAuthorization(new HttpContextWrapper(context));
        }

        private void SetCachePolicy(AuthorizationContext context)
        {
            HttpCachePolicyBase policy = context.HttpContext.Response.Cache;
            policy.SetProxyMaxAge(new TimeSpan(0));
            policy.AddValidationCallback(CacheValidateHandler, null);
        }
    }
}