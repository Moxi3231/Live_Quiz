namespace Live_Quiz.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    public class DataModel : DbContext
    {
        // Your context has been configured to use a 'Model1' connection string from your application's 
        // configuration file (App.config or Web.config). By default, this connection string targets the 
        // 'generateQ.Models.Model1' database on your LocalDb instance. 
        // 
        // If you wish to target a different database and/or database provider, modify the 'Model1' 
        // connection string in the application configuration file.
        public DataModel()
            : base("name=DefaultConnection")
        {
        }



        public virtual DbSet<UserProfile> UserProfiles { get; set; }
        public virtual DbSet<Quiz> Quizs { get; set; }
        public virtual DbSet<QuizQuestion> QuizQuestions { get; set; }
        public virtual DbSet<Question> Questions { get; set; }

        public virtual DbSet<Options> Optionss { get; set; }
        public virtual DbSet<Collection> Collections { get; set; }

        public virtual DbSet<QuizCollection> QuizCollections { get; set; }
        public virtual DbSet<ImageFile> Images { get; set; }
        public virtual DbSet<League> Leagues { get; set; }
        public virtual DbSet<UserQuiz> UserQuizzes { get; set; }
    }
    public class ImageFile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public byte[] Image { get; set; }

        // public Quiz Quiz { get; set; }
        //   public Collection Collection { get; set; }
        //  public Question Question { get; set; }
        //  public Options Options { get; set; }
    }

    public class UserProfile
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string AccountId { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }

        public virtual ICollection<Question> Questions { get; set; }


    }
    public class UserQuiz
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UId { get; set; }
        [Required]
        public int QId { get; set; }
    }
    public class QuizQuestion
    {

        [Key, Column(Order = 1)]
        public int QuizId { get; set; }
        [Key, Column(Order = 2)]
        public int QuestionId { get; set; }
        public Quiz Quiz { get; set; }
        public Question Question { get; set; }

    }
    public class Quiz
    {

        [Key]
        public int Id { get; set; }

        // [ForeignKey("CoverImage")]
        public int ImageId { get; set; }
        // public ImageFile CoverImage { get; set; }
        [Required]
        
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public bool isPublic { get; set; }
        //  [ForeignKey("Collection")]
        //public int CollectionId { get; set; }
        //public virtual Collection Collection { get; set; }
        //// [ForeignKey("User")]
        // public string UserId { get; set; }
        //public virtual ApplicationUser User { get; set; }
        //[ForeignKey("User")]
        [Required]
        public int UPId { get; set; }
       // public UserProfile User { get; set; }
        public virtual ICollection<QuizCollection> QuizCollections { get; set; }
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int Score { get; set; }

        public string Hint { get; set; }

        // [ForeignKey("CoverImage")]
        public int ImageId { get; set; }
        // public ImageFile CoverImage { get; set; }
        [Required]
        public bool isPublic { get; set; }
        [ForeignKey("User")]
        public int UserProfileId { get; set; }
        public UserProfile User { get; set; }
        public virtual ICollection<Options> Optionss { get; set; }

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
    public class Options
    {

        [Key]
        public int OptionsId { get; set; }
        [Required]
        public string ans { get; set; }

        // [ForeignKey("CoverImage")]
        public int ImageId { get; set; }
        // public ImageFile CoverImage { get; set; }
        [Required]
        public bool isAnswer { get; set; }
    }
    public class Collection
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }

        [ForeignKey("User")]
        public int UserProfileId { get; set; }
        public UserProfile User { get; set; }
        [Required]
        public bool isPublic { get; set; }
        [Required]
        public string Description { get; set; }

        //[ForeignKey("User")]
        // public string UserId { get; set; }

        // public virtual ApplicationUser User { get; set; }

        public int ImageId { get; set; }
        //public ImageFile CoverImage { get; set; }

        public virtual ICollection<QuizCollection> QuizeCollections { get; set; }


    }

    public class QuizCollection
    {

        [Key, Column(Order = 1)]
        public int QuizId { get; set; }
        [Key, Column(Order = 2)]
        public int CollectionId { get; set; }
        public Quiz Quiz { get; set; }
        public Collection Collection { get; set; }

    }
    public class League
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string LeagueName { get; set; }
        [Required]
        public int Min_Value { get; set; }
        [Required]
        public int Max_Value { get; set; }
    }
}