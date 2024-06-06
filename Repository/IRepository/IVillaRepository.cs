using MagicVilla_Api.Models;

namespace MagicVilla_Api.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
	Task<Villa> UpdateAsync(Villa entity);
}
