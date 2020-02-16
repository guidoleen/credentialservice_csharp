
namespace CredentialService
{
	public static class CacheControlService
	{
		public static ICacheControl<ClientInformation> GetCacheController()
		{
			var cacheController = new CacheControl<ClientInformation>(); // Can be a db/redis controller
			return cacheController;
		}
	}
}