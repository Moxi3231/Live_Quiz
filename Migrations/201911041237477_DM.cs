namespace Live_Quiz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DM : DbMigration
    {
        public override void Up()
        {
          CreateTable(
                "dbo.Collections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Email = c.String(),
                        UserProfileId = c.Int(nullable: false),
                        isPublic = c.Boolean(nullable: false),
                        Description = c.String(nullable: false),
                        ImageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.QuizCollections",
                c => new
                    {
                        QuizId = c.Int(nullable: false),
                        CollectionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuizId, t.CollectionId })
                .ForeignKey("dbo.Collections", t => t.CollectionId, cascadeDelete: true)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId)
                .Index(t => t.CollectionId);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ImageId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Description = c.String(nullable: false),
                        isPublic = c.Boolean(nullable: false),
                        UPId = c.Int(nullable: false),
                        UserProfile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfile_Id)
                .Index(t => t.UserProfile_Id);
            
            CreateTable(
                "dbo.QuizQuestions",
                c => new
                    {
                        QuizId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.QuizId, t.QuestionId })
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        QuestionId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        Score = c.Int(nullable: false),
                        Hint = c.String(),
                        ImageId = c.Int(nullable: false),
                        isPublic = c.Boolean(nullable: false),
                        UserProfileId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.QuestionId)
                .ForeignKey("dbo.UserProfiles", t => t.UserProfileId, cascadeDelete: true)
                .Index(t => t.UserProfileId);
            
            CreateTable(
                "dbo.Options",
                c => new
                    {
                        OptionsId = c.Int(nullable: false, identity: true),
                        ans = c.String(nullable: false),
                        ImageId = c.Int(nullable: false),
                        isAnswer = c.Boolean(nullable: false),
                        Question_QuestionId = c.Int(),
                    })
                .PrimaryKey(t => t.OptionsId)
                .ForeignKey("dbo.Questions", t => t.Question_QuestionId)
                .Index(t => t.Question_QuestionId);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ImageFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Leagues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeagueName = c.String(nullable: false),
                        Min_Value = c.Int(nullable: false),
                        Max_Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Collections", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.QuizQuestions", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.Questions", "UserProfileId", "dbo.UserProfiles");
            DropForeignKey("dbo.Quizs", "UserProfile_Id", "dbo.UserProfiles");
            DropForeignKey("dbo.QuizQuestions", "QuestionId", "dbo.Questions");
            DropForeignKey("dbo.Options", "Question_QuestionId", "dbo.Questions");
            DropForeignKey("dbo.QuizCollections", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.QuizCollections", "CollectionId", "dbo.Collections");
            DropIndex("dbo.Options", new[] { "Question_QuestionId" });
            DropIndex("dbo.Questions", new[] { "UserProfileId" });
            DropIndex("dbo.QuizQuestions", new[] { "QuestionId" });
            DropIndex("dbo.QuizQuestions", new[] { "QuizId" });
            DropIndex("dbo.Quizs", new[] { "UserProfile_Id" });
            DropIndex("dbo.QuizCollections", new[] { "CollectionId" });
            DropIndex("dbo.QuizCollections", new[] { "QuizId" });
            DropIndex("dbo.Collections", new[] { "UserProfileId" });
            DropTable("dbo.Leagues");
            DropTable("dbo.ImageFiles");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.Options");
            DropTable("dbo.Questions");
            DropTable("dbo.QuizQuestions");
            DropTable("dbo.Quizs");
            DropTable("dbo.QuizCollections");
            DropTable("dbo.Collections");
        }
    }
}
