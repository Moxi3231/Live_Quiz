using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Live_Quiz.Models
{
   
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Linq;

    using System.Web;
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
        public virtual DbSet<Image> Images { get; set; }

    }
    public class Image
    {
        [Key]
        public int Id { get; set; }

        public string file { get; set; }

        public string path { get; set; }

    }

    public class UserProfile
    {
        [Key]
        public int Id { get; set; }

        public string AccountId { get; set; }

        public virtual ICollection<Quiz> Quizzes { get; set; }

        public virtual ICollection<Collection> Collections { get; set; }

        public virtual ICollection<Question> Questions { get; set; }


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

        public Image CoverImage { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public bool isPublic { get; set; }
        public virtual Collection Collection { get; set; }
        // [ForeignKey("User")]
        // public string UserId { get; set; }
        //   public virtual ApplicationUser User { get; set; }
        public virtual ICollection<QuizCollection> QuizCollections { get; set; }
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        public string Description { get; set; }

        public int Score { get; set; }

        public string Hint { get; set; }

        public Image CoverImage { get; set; }

        public bool isPublic { get; set; }

        public virtual ICollection<Options> Optionss { get; set; }

        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
    public class Options
    {

        [Key]
        public int OptionsId { get; set; }

        public string ans { get; set; }

        public Image CoverImage { get; set; }

        public bool isAnswer { get; set; }
    }
    public class Collection
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }


        public bool isPublic { get; set; }

        public string Description { get; set; }

        //[ForeignKey("User")]
        // public string UserId { get; set; }

        // public virtual ApplicationUser User { get; set; }

        public Image CoverImage { get; set; }

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

}