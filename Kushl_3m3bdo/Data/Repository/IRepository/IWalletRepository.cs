using Kushl_3m3bdo.Models;

namespace Kushl_3m3bdo.Data.Repository.IRepository
{
	public interface IWalletRepository : IGenericRepository<Wallet>
	{
		Task Update (Wallet wallet);
	}
}
