using Microsoft.EntityFrameworkCore;
using EstateLinkAvalonia.Models;

namespace EstateLinkAvalonia.Data
{
    public class EstateLinkContext : DbContext
    {
        public EstateLinkContext(DbContextOptions<EstateLinkContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasOne(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeID)
                .IsRequired();

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Property)
                .WithMany()
                .HasForeignKey(o => o.PropertyID)
                .IsRequired();

            modelBuilder.Entity<Offer>()
                .HasOne(o => o.Realtor)
                .WithMany()
                .HasForeignKey(o => o.RealtorID)
                .IsRequired();

            modelBuilder.Entity<Need>()
                .HasOne(n => n.Client)
                .WithMany()
                .HasForeignKey(n => n.ClientID)
                .IsRequired();

            modelBuilder.Entity<Need>()
                .HasOne(n => n.Realtor)
                .WithMany()
                .HasForeignKey(n => n.RealtorID)
                .IsRequired();

            modelBuilder.Entity<Need>()
                .HasOne(n => n.Property)
                .WithMany()
                .HasForeignKey(n => n.PropertyID)
                .IsRequired();

            modelBuilder.Entity<Need>()
                .HasOne(n => n.ApartmentNeed)
                .WithOne(an => an.Need)
                .HasForeignKey<ApartmentNeed>(an => an.NeedID);

            modelBuilder.Entity<Need>()
                .HasOne(n => n.HouseNeed)
                .WithOne(hn => hn.Need)
                .HasForeignKey<HouseNeed>(hn => hn.NeedID);

            modelBuilder.Entity<Need>()
                .HasOne(n => n.LandNeed)
                .WithOne(ln => ln.Need)
                .HasForeignKey<LandNeed>(ln => ln.NeedID);
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Realtor> Realtors { get; set; } = null!;
        public DbSet<Property> Properties { get; set; } = null!;
        public DbSet<PropertyType> PropertyTypes { get; set; } = null!;
        public DbSet<Offer> Offers { get; set; } = null!;
        public DbSet<Need> Needs { get; set; } = null!;
        public DbSet<LandNeed> LandNeeds { get; set; } = null!;
        public DbSet<HouseNeed> HouseNeeds { get; set; } = null!;
        public DbSet<ApartmentNeed> ApartmentNeeds { get; set; } = null!;
    }
} 