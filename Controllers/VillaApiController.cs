﻿using MagicVilla_Api.Data;
using MagicVilla_Api.Logging;
using MagicVilla_Api.Models;
using MagicVilla_Api.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_Api.Controllers;

//[Route("api/[controller]")]
[Route("api/VillaAPI")]
[ApiController]
public class VillaApiController : ControllerBase
{
	private readonly ApplicationDbContext _db;
	private readonly ILogging _logger;

	public VillaApiController(ApplicationDbContext context, ILogging logger)
	{
		_db = context;
		_logger = logger;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
	{
		_logger.Log("Getting all villas", "");
		return Ok(await _db.Villas.ToListAsync());
	}

	[HttpGet("{id:int}", Name = "GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	//[ProducesResponseType(200, Type = typeof(VillaDto))]
	public async Task<ActionResult<VillaDto>> GetVilla(int id)
	{
		if (id == 0)
		{
			_logger.Log("Get villa error with id " + id, "error");
			return BadRequest();
		}

		var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

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
	public async Task<ActionResult<VillaDto>> CreateVilla([FromBody] VillaCreateDto villa)
	{
		if (await _db.Villas.FirstOrDefaultAsync(u => u.Name.ToLower() == villa.Name.ToLower()) != null)
		{
			ModelState.AddModelError("CustomError", "Villa already exists!");
			return BadRequest(ModelState);
		}

		if (villa == null)
		{
			return BadRequest(villa);
		}

		Villa model = new()
		{
			Amenity = villa.Amenity,
			Details = villa.Details,
			ImageUrl = villa.ImageUrl,
			Name = villa.Name,
			Occupancy = villa.Occupancy,
			Rate = villa.Rate,
			Sqft = villa.Sqft,
		};

		await _db.Villas.AddAsync(model);
		await _db.SaveChangesAsync();

		return CreatedAtRoute("GetVilla", new { id = model.Id }, model);
	}

	[HttpDelete("{id:int}", Name = "DeleteVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteVilla(int id)
	{
		if (id == 0)
		{
			return BadRequest();
		}

		var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);

		if (villa == null)
		{
			return NotFound();
		}

		_db.Villas.Remove(villa);
		await _db.SaveChangesAsync();

		return NoContent();
	}

	[HttpPut("{id:int}", Name = "UpdateVilla")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto villaDto)
	{
		if (villaDto == null || id != villaDto.Id)
		{
			return BadRequest();
		}

		Villa model = new()
		{
			Amenity = villaDto.Amenity,
			Details = villaDto.Details,
			Id = villaDto.Id,
			ImageUrl = villaDto.ImageUrl,
			Name = villaDto.Name,
			Occupancy = villaDto.Occupancy,
			Rate = villaDto.Rate,
			Sqft = villaDto.Sqft,
		};

		_db.Villas.Update(model);
		await _db.SaveChangesAsync();

		return NoContent();
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

		var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

		VillaUpdateDto villaDto = new()
		{
			Amenity = villa.Amenity,
			Details = villa.Details,
			Id = villa.Id,
			ImageUrl = villa.ImageUrl,
			Name = villa.Name,
			Occupancy = villa.Occupancy,
			Rate = villa.Rate,
			Sqft = villa.Sqft,
		};

		if (villa == null)
		{
			return BadRequest();
		}

		patchDto.ApplyTo(villaDto, ModelState);

		Villa model = new()
		{
			Amenity = villaDto.Amenity,
			Details = villaDto.Details,
			Id = villaDto.Id,
			ImageUrl = villaDto.ImageUrl,
			Name = villaDto.Name,
			Occupancy = villaDto.Occupancy,
			Rate = villaDto.Rate,
			Sqft = villaDto.Sqft,
		};

		_db.Villas.Update(model);
		await _db.SaveChangesAsync();

		if (!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		return NoContent();
	}
}
