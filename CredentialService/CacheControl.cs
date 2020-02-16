using System;
using System.Web.Caching;

namespace CredentialService
{
	public class CacheControl<T> : ICacheControl<T>
	{
		private static System.Web.Caching.Cache _cache = CacheControlCache.GetCache();

		public object GetCache()
		{
			return (Cache)_cache;
		}

		public void AddItem(string itemName, T obj)
		{
			_cache.Add(
					itemName,
					obj,
					null,
					Cache.NoAbsoluteExpiration, // DateTime.Now.AddMinutes(AppConfigurationSettings.CacheExpiration),
					TimeSpan.FromMinutes(AppConfigurationSettings.CacheExpiration), // Cache.NoSlidingExpiration,
					CacheItemPriority.Default,
					null
				);
		}

		public int GetCacheLenght()
		{
			return _cache.Count;
		}

		public void RemoveItem(string itemName)
		{
			_cache.Remove(itemName);
		}

		public T GetItem(string itemName)
		{
			return (T)_cache.Get(itemName);
		}

		public void EmptyCache()
		{
			var cache = _cache;
			var cacheEnumerator = cache.GetEnumerator();
			if (cache == null) return;

			while (cacheEnumerator.MoveNext())
			{
				cache.Remove(cacheEnumerator.Key.ToString());
			}
		}
		private void onRemoveCallback(string key, object value, CacheItemRemovedReason reason)
		{
			throw new NotImplementedException();
		}
	}
}