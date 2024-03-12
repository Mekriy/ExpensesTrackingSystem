using EST.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EST.DAL.DataAccess.EF.Configurations;

public class PhotoFileConfiguration : IEntityTypeConfiguration<PhotoFile>
{
    public void Configure(EntityTypeBuilder<PhotoFile> builder)
    {
        builder
            .HasKey(l => l.Id);
        builder
            .HasOne(u => u.User)
            .WithOne(u => u.PhotoFile)
            .HasForeignKey<PhotoFile>(u => u.UserId);
    }
}