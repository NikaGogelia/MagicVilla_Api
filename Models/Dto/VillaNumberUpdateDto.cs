﻿using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Api.Models.Dto;

public class VillaNumberUpdateDto
{
	[Required]
	public int VillaNo { get; set; }
	public string SpecialDetails { get; set; }
}
