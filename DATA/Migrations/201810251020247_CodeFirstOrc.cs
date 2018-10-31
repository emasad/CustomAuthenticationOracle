namespace DATA.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CodeFirstOrc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "SBSOFT.Roles",
                c => new
                    {
                        RoleId = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
                        RoleName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.RoleId);
            
            CreateTable(
                "SBSOFT.Users",
                c => new
                    {
                        UserId = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
                        Username = c.String(maxLength: 100),
                        FirstName = c.String(maxLength: 100, unicode: false),
                        LastName = c.String(maxLength: 50, unicode: false),
                        Email = c.String(maxLength: 50, unicode: false),
                        Password = c.String(maxLength: 50, unicode: false),
                        IsActive = c.Decimal(nullable: false, precision: 1, scale: 0),
                        ActivationCode = c.String(maxLength: 256, unicode: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "SBSOFT.UserRoles",
                c => new
                    {
                        UserId = c.Decimal(nullable: false, precision: 10, scale: 0),
                        RoleId = c.Decimal(nullable: false, precision: 10, scale: 0),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("SBSOFT.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("SBSOFT.Roles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("SBSOFT.UserRoles", "RoleId", "SBSOFT.Roles");
            DropForeignKey("SBSOFT.UserRoles", "UserId", "SBSOFT.Users");
            DropIndex("SBSOFT.UserRoles", new[] { "RoleId" });
            DropIndex("SBSOFT.UserRoles", new[] { "UserId" });
            DropTable("SBSOFT.UserRoles");
            DropTable("SBSOFT.Users");
            DropTable("SBSOFT.Roles");
        }
    }
}
