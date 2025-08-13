using EnterpriseAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseAPI.Data
{
	public class AppDbContext : DbContext
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		public DbSet<Enterprise> Enterprises => Set<Enterprise>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Enterprise>(builder =>
			{
				builder.HasKey(e => e.Id);
				builder.OwnsOne(e => e.TaxAddress, ta =>
				{
					ta.Property(p => p.Province).HasMaxLength(100);
					ta.Property(p => p.District).HasMaxLength(100);
				});
			});

			base.OnModelCreating(modelBuilder);
		}
	}
} 