namespace Live_Quiz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBModel3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserQuizs", "Score", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserQuizs", "Score");
        }
    }
}
