using MagicVilla_Api.Data;
using MagicVilla_Api.Logging;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_Api.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaApiController : ControllerBase
{
	private readonly ILogging _logger;

	public VillaApiController(ILogging logger)
	{
		_logger = logger;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public ActionResult<IEnumerable<VillaDto>> GetVillas()
	{
		_logger.Log("Getting all villas", "");
		return Ok(VillaStore.villaList);
	}

	[HttpGet("{id:int}", Name = "GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(200, Type = typeof(VillaDto))]
	public ActionResult<VillaDto> GetVilla(int id)
	{
		if (id == 0)
		{
			_logger.Log("Get villa error with id " + id, "error");
			return BadRequest();
		}

		var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

		if (villa == null)
		{
			return NotFound();
		}

		return Ok(villa);
	}

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public ActionResult<VillaDto> CreateVilla([FromBody] VillaDto villa)
	{
		//if (!ModelState.IsValid)
		//{
		//	return BadRequest(ModelState);
		//}

		if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
		{
			ModelState.AddModelError("CustomError", "Villa already exists!");
			return BadRequest(ModelState);
		}

		if (villa == null)
		{
			return BadRequest(villa);
		}

		if (villa.Id > 0)
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		villa.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
		VillaStore.villaList.Add(villa);

		return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
	}

	[HttpDelete("{id:int}", Name = "DeleteVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public IActionResult DeleteVilla(int id)
	{
		if (id == 0)
		{
			return BadRequest();
		}

		var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

		if (villa == null)
		{
			return NotFound();
		}

		VillaStore.villaList.Remove(villa);
		return NoContent();
	}

	[HttpPut("{id:int}", Name = "UpdateVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult UpdateVilla(int id, [FromBody] VillaDto villaDto)
	{
		if (villaDto == null || id != villaDto.Id)
		{
			return BadRequest();
		}

		var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

		villa.Name = villaDto.Name;
		villa.Sqft = villaDto.Sqft;
		villa.Occupancy = villaDto.Occupancy;

		return NoContent();
	}

	[HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDto)
	{
		if (patchDto == null || id == 0)
		{
			return BadRequest();
		}

		var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);

		if (villa == null)
		{
			return BadRequest();
		}

		patchDto.ApplyTo(villa, ModelState);

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		return NoContent();
	}
}
