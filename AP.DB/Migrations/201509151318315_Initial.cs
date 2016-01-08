namespace AP.DB.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AstroObjects",
                c => new
                    {
                        Uid = c.String(nullable: false, maxLength: 255),
                        BigPhotoUrl = c.String(),
                        BigPhotoX = c.Int(nullable: false),
                        BigPhotoY = c.Int(nullable: false),
                        CroppedPhotoUrl = c.String(),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Uid);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AstroObjects");
        }
    }
}
