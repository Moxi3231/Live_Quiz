namespace Live_Quiz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBModel2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserQuizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UId = c.Int(nullable: false),
                        QId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserQuizs");
        }
    }
}
