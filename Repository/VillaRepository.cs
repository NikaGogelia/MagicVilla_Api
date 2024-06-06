using MagicVilla_Api.Data;
using MagicVilla_Api.Models;
using MagicVilla_Api.Repository.IRepository;

namespace MagicVilla_Api.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
	private readonly ApplicationDbContext _db;

	public VillaRepository(ApplicationDbContext db) : base(db)
	{
		_db = db;
	}

	public async Task<Villa> UpdateAsync(Villa entity)
	{
		_db.Villas.Update(entity);
		await _db.SaveChangesAsync();
		return entity;
	}
}
