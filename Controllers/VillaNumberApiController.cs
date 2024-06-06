using AutoMapper;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_Api.Controllers;

[Route("api/VillaNumberAPI")]
[ApiController]
public class VillaNumberApiController : ControllerBase
{
	private readonly IVillaNumberRepository _villaNumberRepository;
	private readonly IMapper _mapper;
	protected APIResponse _response;

	public VillaNumberApiController(IVillaNumberRepository dbVillaNumber, IMapper mapper)
	{
		_villaNumberRepository = dbVillaNumber;
		_mapper = mapper;
		this._response = new();
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<APIResponse>> GetVillaNumbers()
	{
		try
		{

			IEnumerable<VillaNumber> villaNumList = await _villaNumberRepository.GetAllAsync();
			_response.Result = _mapper.Map<List<VillaNumberDto>>(villaNumList);
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpGet("{id:int}", Name = "GetVillaNumber")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
	{
		try
		{

			if (id == 0)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_response);
			}

			var villaNum = await _villaNumberRepository.GetAsync(u => u.VillaNo == id);

			if (villaNum == null)
			{
				_response.StatusCode = HttpStatusCode.NotFound;
				return NotFound(_response);
			}

			_response.Result = _mapper.Map<VillaNumberDto>(villaNum);
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
	{
		try
		{

			if (await _villaNumberRepository.GetAsync(u => u.SpecialDetails.ToLower() == createDto.SpecialDetails.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already exists!");
				return BadRequest(ModelState);
			}

			if (createDto == null)
			{
				return BadRequest(createDto);
			}

			VillaNumber model = _mapper.Map<VillaNumber>(createDto);

			await _villaNumberRepository.CreateAsync(model);

			_response.Result = _mapper.Map<VillaNumberDto>(model);
			_response.StatusCode = HttpStatusCode.Created;

			return CreatedAtRoute("GetVillaNumber", new { id = model.VillaNo }, _response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
	{
		try
		{
			if (id == 0)
			{
				return BadRequest();

			}

			var villaNum = await _villaNumberRepository.GetAsync(u => u.VillaNo == id);

			if (villaNum == null)
			{
				return NotFound();
			}

			await _villaNumberRepository.RemoveAsync(villaNum);
			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;

			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpPut("{id:int}", Name = "UpdateVillaNumber")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
	{
		try
		{

			if (updateDto == null || id != updateDto.VillaNo)
			{
				return BadRequest();
			}

			VillaNumber model = _mapper.Map<VillaNumber>(updateDto);

			await _villaNumberRepository.UpdateAsync(model);

			_response.StatusCode = HttpStatusCode.NoContent;
			_response.IsSuccess = true;

			return Ok(_response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}
}
