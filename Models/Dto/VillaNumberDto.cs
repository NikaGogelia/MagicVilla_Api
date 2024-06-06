using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Models.Dto;

public class VillaNumberDto
{
	[Required]
	public int VillaNot { get; set; }
	public string SpecialDetails { get; set; }
}
