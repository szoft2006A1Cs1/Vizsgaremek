using JogsifoglaloApi.Data;
using Microsoft.EntityFrameworkCore;
using System;


namespace JogsifoglaloApi.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static JogsifoglaloContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<JogsifoglaloContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new JogsifoglaloContext(options);
            databaseContext.Database.EnsureCreated();

            return databaseContext;
        }
    }
}
