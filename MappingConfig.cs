using AutoMapper;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;

namespace MagicVilla_Api;

public class MappingConfig : Profile
{
	public MappingConfig()
	{
		// Villa Mapping
		CreateMap<Villa, VillaDto>().ReverseMap();
		CreateMap<Villa, VillaCreateDto>().ReverseMap();
		CreateMap<Villa, VillaUpdateDto>().ReverseMap();

		// VillaNumber Mapping
		CreateMap<VillaNumber, VillaNumberDto>().ReverseMap();
		CreateMap<VillaNumber, VillaNumberCreateDto>().ReverseMap();
		CreateMap<VillaNumber, VillaNumberUpdateDto>().ReverseMap();
	}
}
