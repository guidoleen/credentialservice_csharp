
namespace CredentialService
{
	public interface ICacheControl<T>
	{
		object GetCache();
		void AddItem(string itemName, T obj);
		void RemoveItem(string itemName);
		T GetItem(string itemName);
		void EmptyCache();
		int GetCacheLenght();
	}
}
