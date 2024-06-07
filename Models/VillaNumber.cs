using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MagicVilla_Api.Models;

public class VillaNumber
{
	[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
	public int VillaNo { get; set; }
	public string SpecialDetails { get; set; }

	public int VillaId { get; set; }
	[ForeignKey(nameof(VillaId))]
	public Villa Villa { get; set; }

	public DateTime CreatedDate { get; set; }
	public DateTime UpdatedDate { get; set; }
}
