using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircusGroupsBot.Services
{
    class CircusDBContextFactory : IDesignTimeDbContextFactory<CircusDbContext>
    {
        public CircusDbContext CreateDbContext(string[] args)
        {
            var dbPass = Environment.GetEnvironmentVariable(Config.DB_PASSWORD_ENV_VAR);

            var optionsBuilder = new DbContextOptionsBuilder<CircusDbContext>();
            optionsBuilder.UseMySql($"server=localhost;user=root;password={dbPass};database=circusdb",
                        mySqlOptions => mySqlOptions
                            .ServerVersion(new Version(10, 3, 27), ServerType.MariaDb)
                            .CharSetBehavior(CharSetBehavior.NeverAppend));

            return new CircusDbContext(optionsBuilder.Options);
        }
    }
}
