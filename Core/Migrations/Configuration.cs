namespace Core.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Core.Model;

    public sealed class Configuration : DbMigrationsConfiguration<Core.Persistence.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Core.Persistence.Context context)
        {
            if (!context.Users.Any())
            {
                var user = new User() { Email = "demo@pjotr.info", Password = "test", ApiKey = "bWFpbEBwZXRlcmdlcnJpdHNlbi5ubA" };

                context.Users.Add(user);

                context.SaveChanges();
            }                        
        }
    }
}
