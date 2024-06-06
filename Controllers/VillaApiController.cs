using AutoMapper;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using MagicVilla_Api.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_Api.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaApiController : ControllerBase
{
	private readonly IVillaRepository _villaRepository;
	private readonly IMapper _mapper;
	protected APIResponse _response;

	public VillaApiController(IVillaRepository dbVilla, IMapper mapper)
	{
		_villaRepository = dbVilla;
		_mapper = mapper;
		this._response = new();
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<APIResponse>> GetVillas()
	{
		try
		{

			IEnumerable<Villa> villaList = await _villaRepository.GetAllAsync();
			_response.Result = _mapper.Map<List<VillaDto>>(villaList);
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

	[HttpGet("{id:int}", Name = "GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(200, Type = typeof(VillaDto))]
	public async Task<ActionResult<APIResponse>> GetVilla(int id)
	{
		try
		{

			if (id == 0)
			{
				_response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_response);
			}

			var villa = await _villaRepository.GetAsync(u => u.Id == id);

			if (villa == null)
			{
				_response.StatusCode = HttpStatusCode.NotFound;
				return NotFound(_response);
			}

			_response.Result = _mapper.Map<VillaDto>(villa);
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
	public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
	{
		try
		{

			if (await _villaRepository.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower()) != null)
			{
				ModelState.AddModelError("CustomError", "Villa already exists!");
				return BadRequest(ModelState);
			}

			if (createDto == null)
			{
				return BadRequest(createDto);
			}

			Villa model = _mapper.Map<Villa>(createDto);

			await _villaRepository.CreateAsync(model);

			_response.Result = _mapper.Map<VillaDto>(model);
			_response.StatusCode = HttpStatusCode.Created;

			return CreatedAtRoute("GetVilla", new { id = model.Id }, _response);
		}
		catch (Exception ex)
		{
			_response.IsSuccess = false;
			_response.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return _response;
	}

	[HttpDelete("{id:int}", Name = "DeleteVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
	{
		try
		{
			if (id == 0)
			{
				return BadRequest();

			}

			var villa = await _villaRepository.GetAsync(u => u.Id == id);

			if (villa == null)
			{
				return NotFound();
			}

			await _villaRepository.RemoveAsync(villa);
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

	[HttpPut("{id:int}", Name = "UpdateVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
	{
		try
		{

			if (updateDto == null || id != updateDto.Id)
			{
				return BadRequest();
			}

			Villa model = _mapper.Map<Villa>(updateDto);

			await _villaRepository.UpdateAsync(model);

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

	[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
	{
		if (patchDto == null || id == 0)
		{
			return BadRequest();
		}

		var villa = await _villaRepository.GetAsync(u => u.Id == id, tracked: false);

		VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

		if (villa == null)
		{
			return BadRequest();
		}

		patchDto.ApplyTo(villaDto, ModelState);

		Villa model = _mapper.Map<Villa>(villaDto);

		_villaRepository.UpdateAsync(model);

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		return NoContent();
	}
}
