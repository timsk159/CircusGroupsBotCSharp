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
            var userName = Environment.GetEnvironmentVariable(Config.DB_USER_ENV_VAR);

            if (string.IsNullOrEmpty(userName))
                userName = "root";

            var optionsBuilder = new DbContextOptionsBuilder<CircusDbContext>();
            optionsBuilder.UseMySql($"server=localhost;user={userName};password={dbPass};database=circusdb;charset=utf8mb4",
                        mySqlOptions => mySqlOptions
                            .ServerVersion(new Version(10, 3, 27), ServerType.MariaDb)
                            .CharSetBehavior(CharSetBehavior.NeverAppend));

            return new CircusDbContext(optionsBuilder.Options);
        }
    }
}
