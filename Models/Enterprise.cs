using System.ComponentModel.DataAnnotations;

namespace EnterpriseAPI.Models
{
	public class Enterprise
	{
		[Key]
		[Required]
		public string Id { get; set; } = Guid.NewGuid().ToString();

		[Required]
		[MinLength(2)]
		[MaxLength(200)]
		public string Title { get; set; } = string.Empty;

		[Required]
		[RegularExpression("^90\\d{10}$", ErrorMessage = "Phone must start with 90 and have 12 digits total")] 
		public string Phone { get; set; } = string.Empty;

		[Required]
		[EmailAddress]
		public string Email { get; set; } = string.Empty;

		[Range(typeof(decimal), "0", "79228162514264337593543950335")]
		[RegularExpression(@"^\\d+(\\.\\d{1,2})?$", ErrorMessage = "Balance must have at most two decimal places")]
		public decimal Balance { get; set; }

		public bool Verified { get; set; } = true;

		[Required]
		[MinLength(5)]
		public string Address { get; set; } = string.Empty;

		[Required]
		[RegularExpression("^\\d{10}$", ErrorMessage = "Tax number must be 10 digits")]
		public long TaxNumber { get; set; }

		[Required]
		public TaxAddress TaxAddress { get; set; } = new();

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public bool Disabled { get; set; } = false;
	}

	public class TaxAddress
	{
		[Required]
		[MinLength(2)]
		[MaxLength(100)]
		public string Province { get; set; } = string.Empty;

		[Required]
		[MinLength(2)]
		[MaxLength(100)]
		public string District { get; set; } = string.Empty;
	}
} 