namespace Live_Quiz.Migrations
{
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<Live_Quiz.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Live_Quiz.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }

    internal sealed class DMConfiguration : DbMigrationsConfiguration<Live_Quiz.Models.DataModel>
    {
        public DMConfiguration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Live_Quiz.Models.DataModel context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
        }
    }
}
