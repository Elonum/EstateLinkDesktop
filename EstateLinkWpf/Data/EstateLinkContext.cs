using System.Data.Entity;
using System.Data.Entity.Migrations;
using EstateLinkWpf.Models;

namespace EstateLinkWpf.Data
{
    public class EstateLinkContext : DbContext
    {
        public EstateLinkContext() : base("name=EstateLinkContext")
        {
            Database.SetInitializer(new EstateLinkInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Property>()
                .HasRequired(p => p.PropertyType)
                .WithMany(pt => pt.Properties)
                .HasForeignKey(p => p.PropertyTypeID);

            modelBuilder.Entity<Offer>()
                .HasRequired(o => o.Property)
                .WithMany()
                .HasForeignKey(o => o.PropertyID);

            modelBuilder.Entity<Offer>()
                .HasRequired(o => o.Realtor)
                .WithMany()
                .HasForeignKey(o => o.RealtorID);

            modelBuilder.Entity<Need>()
                .HasRequired(n => n.Client)
                .WithMany()
                .HasForeignKey(n => n.ClientID);

            modelBuilder.Entity<Need>()
                .HasRequired(n => n.Realtor)
                .WithMany()
                .HasForeignKey(n => n.RealtorID);

            modelBuilder.Entity<Need>()
                .HasRequired(n => n.Property)
                .WithMany()
                .HasForeignKey(n => n.PropertyID);

            modelBuilder.Entity<Need>()
                .HasOptional(n => n.ApartmentNeed)
                .WithRequired(an => an.Need);

            modelBuilder.Entity<Need>()
                .HasOptional(n => n.HouseNeed)
                .WithRequired(hn => hn.Need);

            modelBuilder.Entity<Need>()
                .HasOptional(n => n.LandNeed)
                .WithRequired(ln => ln.Need);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Realtor> Realtors { get; set; }
        public DbSet<Property> Properties { get; set; }
        public DbSet<PropertyType> PropertyTypes { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Need> Needs { get; set; }
        public DbSet<LandNeed> LandNeeds { get; set; }
        public DbSet<HouseNeed> HouseNeeds { get; set; }
        public DbSet<ApartmentNeed> ApartmentNeeds { get; set; }
    }

    public class EstateLinkInitializer : CreateDatabaseIfNotExists<EstateLinkContext> {}
}
