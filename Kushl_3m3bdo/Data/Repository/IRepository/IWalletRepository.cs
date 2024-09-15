using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IWalletRepository
	{
		IEnumerable<Wallet> GetAll();
		Wallet GetById(int id);

		void Insert (Wallet wallet);
		void Update (int Id, Wallet wallet);
		void Delete (int Id);
	}
}
