namespace EstateLinkWpf.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ApartmentNeeds",
                c => new
                    {
                        NeedID = c.Int(nullable: false),
                        MinRoomCount = c.Int(nullable: false),
                        MaxRoomCount = c.Int(nullable: false),
                        MinFloor = c.Int(nullable: false),
                        MaxFloor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NeedID)
                .ForeignKey("dbo.Needs", t => t.NeedID)
                .Index(t => t.NeedID);
            
            CreateTable(
                "dbo.Needs",
                c => new
                    {
                        NeedID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        RealtorID = c.Int(nullable: false),
                        PropertyID = c.Int(nullable: false),
                        MinPrice = c.Int(nullable: false),
                        MaxPrice = c.Int(nullable: false),
                        MinArea = c.Double(nullable: false),
                        MaxArea = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.NeedID)
                .ForeignKey("dbo.Clients", t => t.ClientID, cascadeDelete: true)
                .ForeignKey("dbo.Property", t => t.PropertyID, cascadeDelete: true)
                .ForeignKey("dbo.Realtors", t => t.RealtorID, cascadeDelete: true)
                .Index(t => t.ClientID)
                .Index(t => t.RealtorID)
                .Index(t => t.PropertyID);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Patronymic = c.String(),
                        Phone = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.HouseNeeds",
                c => new
                    {
                        NeedID = c.Int(nullable: false),
                        MinRoomCount = c.Int(nullable: false),
                        MaxRoomCount = c.Int(nullable: false),
                        MinFloor = c.Int(nullable: false),
                        MaxFloor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NeedID)
                .ForeignKey("dbo.Needs", t => t.NeedID)
                .Index(t => t.NeedID);
            
            CreateTable(
                "dbo.LandNeeds",
                c => new
                    {
                        NeedID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.NeedID)
                .ForeignKey("dbo.Needs", t => t.NeedID)
                .Index(t => t.NeedID);
            
            CreateTable(
                "dbo.Property",
                c => new
                    {
                        PropertyID = c.Int(nullable: false, identity: true),
                        City = c.String(),
                        Street = c.String(),
                        House = c.String(),
                        Apartment = c.String(),
                        Latitude = c.Double(),
                        Longitude = c.Double(),
                        Floor = c.Int(),
                        RoomCount = c.Int(),
                        Area = c.Double(),
                        PropertyTypeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyID)
                .ForeignKey("dbo.PropertyType", t => t.PropertyTypeID, cascadeDelete: true)
                .Index(t => t.PropertyTypeID);
            
            CreateTable(
                "dbo.PropertyType",
                c => new
                    {
                        PropertyTypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PropertyTypeID);
            
            CreateTable(
                "dbo.Realtors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Patronymic = c.String(nullable: false),
                        CommissionShare = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Offer",
                c => new
                    {
                        OfferID = c.Int(nullable: false, identity: true),
                        ClientID = c.Int(nullable: false),
                        RealtorID = c.Int(nullable: false),
                        PropertyID = c.Int(nullable: false),
                        Price = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.OfferID)
                .ForeignKey("dbo.Clients", t => t.ClientID, cascadeDelete: true)
                .ForeignKey("dbo.Property", t => t.PropertyID, cascadeDelete: true)
                .ForeignKey("dbo.Realtors", t => t.RealtorID, cascadeDelete: true)
                .Index(t => t.ClientID)
                .Index(t => t.RealtorID)
                .Index(t => t.PropertyID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Offer", "RealtorID", "dbo.Realtors");
            DropForeignKey("dbo.Offer", "PropertyID", "dbo.Property");
            DropForeignKey("dbo.Offer", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.Needs", "RealtorID", "dbo.Realtors");
            DropForeignKey("dbo.Needs", "PropertyID", "dbo.Property");
            DropForeignKey("dbo.Property", "PropertyTypeID", "dbo.PropertyType");
            DropForeignKey("dbo.LandNeeds", "NeedID", "dbo.Needs");
            DropForeignKey("dbo.HouseNeeds", "NeedID", "dbo.Needs");
            DropForeignKey("dbo.Needs", "ClientID", "dbo.Clients");
            DropForeignKey("dbo.ApartmentNeeds", "NeedID", "dbo.Needs");
            DropIndex("dbo.Offer", new[] { "PropertyID" });
            DropIndex("dbo.Offer", new[] { "RealtorID" });
            DropIndex("dbo.Offer", new[] { "ClientID" });
            DropIndex("dbo.Property", new[] { "PropertyTypeID" });
            DropIndex("dbo.LandNeeds", new[] { "NeedID" });
            DropIndex("dbo.HouseNeeds", new[] { "NeedID" });
            DropIndex("dbo.Needs", new[] { "PropertyID" });
            DropIndex("dbo.Needs", new[] { "RealtorID" });
            DropIndex("dbo.Needs", new[] { "ClientID" });
            DropIndex("dbo.ApartmentNeeds", new[] { "NeedID" });
            DropTable("dbo.Offer");
            DropTable("dbo.Realtors");
            DropTable("dbo.PropertyType");
            DropTable("dbo.Property");
            DropTable("dbo.LandNeeds");
            DropTable("dbo.HouseNeeds");
            DropTable("dbo.Clients");
            DropTable("dbo.Needs");
            DropTable("dbo.ApartmentNeeds");
        }
    }
}
