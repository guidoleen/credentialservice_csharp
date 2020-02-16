
namespace CredentialService
{
	public static class CacheControlCache
	{
		private static System.Web.Caching.Cache _cache = new System.Web.Caching.Cache();
		public static System.Web.Caching.Cache GetCache()
		{
			return _cache;
		}
	}
}