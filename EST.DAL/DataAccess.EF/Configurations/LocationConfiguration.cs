using EST.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EST.DAL.DataAccess.EF.Configurations
{
    public class LocationConfiguration : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder
                .HasKey(l => l.Id);
            builder
                .HasMany(e => e.Expenses)
                .WithOne(l => l.Location);
        }
    }
}
