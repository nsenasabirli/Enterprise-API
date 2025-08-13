using EnterpriseAPI.Data;
using EnterpriseAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Globalization;

namespace EnterpriseAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class EnterprisesController : ControllerBase
	{
		private readonly AppDbContext _db;

		public EnterprisesController(AppDbContext db)
		{
			_db = db;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var items = await _db.Enterprises.AsNoTracking().OrderByDescending(e => e.CreatedAt).ToListAsync();
			return Ok(new { status = "ok", message = "Fetched", data = items });
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById([FromRoute] string id)
		{
			var entity = await _db.Enterprises.FindAsync(id);
			if (entity == null)
			{
				return NotFound(new ProblemDetails { Detail = "Enterprise not found" });
			}
			return Ok(new { status = "ok", message = "Fetched", data = entity });
		}

		public class CreateEnterpriseRequest
		{
			[Required]
			[MinLength(2), MaxLength(200)]
			[JsonPropertyName("title")]
			public string Title { get; set; } = string.Empty;

			[Required]
			[RegularExpression("^90\\d{10}$")]
			[JsonPropertyName("phone")]
			public string Phone { get; set; } = string.Empty;

			[Required]
			[EmailAddress]
			[JsonPropertyName("email")]
			public string Email { get; set; } = string.Empty;

			[Required]
			[RegularExpression(@"^\d+([\.,]\d{1,2})?$")]
			[JsonPropertyName("balance")]
			public string Balance { get; set; } = "0";

			[Required]
			[MinLength(5)]
			[JsonPropertyName("address")]
			public string Address { get; set; } = string.Empty;

			[Required]
			[RegularExpression("^\\d{10}$")]
			[JsonPropertyName("tax_number")]
			public string TaxNumber { get; set; } = string.Empty;

			[Required]
			[JsonPropertyName("tax_address")]
			public TaxAddress TaxAddress { get; set; } = new();
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreateEnterpriseRequest req)
		{
			if (!ModelState.IsValid)
			{
				return ValidationProblem(ModelState);
			}

			var normalizedBalance = req.Balance.Replace(',', '.');
			if (!decimal.TryParse(normalizedBalance, NumberStyles.Number, CultureInfo.InvariantCulture, out var balance))
			{
				return BadRequest(new ProblemDetails { Detail = "Invalid balance" });
			}

			var entity = new Enterprise
			{
				Id = Guid.NewGuid().ToString(),
				Title = req.Title.Trim(),
				Phone = req.Phone,
				Email = req.Email.Trim(),
				Balance = Math.Round(balance, 2, MidpointRounding.AwayFromZero),
				Verified = true,
				Address = req.Address.Trim(),
				TaxNumber = long.Parse(req.TaxNumber),
				TaxAddress = new TaxAddress
				{
					Province = req.TaxAddress.Province.Trim(),
					District = req.TaxAddress.District.Trim()
				},
				CreatedAt = DateTime.UtcNow,
				Disabled = false
			};

			_db.Enterprises.Add(entity);
			await _db.SaveChangesAsync();

			return Ok(new { status = "ok", message = "Created", data = entity });
		}

		[HttpPatch("{id}/toggle")]
		public async Task<IActionResult> ToggleDisabled([FromRoute] string id)
		{
			var entity = await _db.Enterprises.FindAsync(id);
			if (entity == null)
			{
				return NotFound(new ProblemDetails { Detail = "Enterprise not found" });
			}
			entity.Disabled = !entity.Disabled;
			await _db.SaveChangesAsync();
			return Ok(new { status = "ok", message = entity.Disabled ? "Disabled" : "Enabled", data = entity });
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete([FromRoute] string id)
		{
			var entity = await _db.Enterprises.FindAsync(id);
			if (entity == null)
			{
				return NotFound(new ProblemDetails { Detail = "Enterprise not found" });
			}
			_db.Enterprises.Remove(entity);
			await _db.SaveChangesAsync();
			return Ok(new { status = "ok", message = "Deleted", data = id });
		}
	}
} 