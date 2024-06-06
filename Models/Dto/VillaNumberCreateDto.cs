using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Models.Dto;

public class VillaNumberCreateDto
{
	[Required]
	public int VillaNot { get; set; }
	public string SpecialDetails { get; set; }
}
