using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SweetSavoryTreats.Models
{
  public class SweetSavoryTreatsContextFactory : IDesignTimeDbContextFactory<SweetSavoryTreatsContext>
  {

    SweetSavoryTreatsContext IDesignTimeDbContextFactory<SweetSavoryTreatsContext>.CreateDbContext(string[] args)
    {
      IConfigurationRoot configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile("appsettings.json")
          .Build();

      var builder = new DbContextOptionsBuilder<SweetSavoryTreatsContext>();
      var connectionString = configuration.GetConnectionString("DefaultConnection");

      builder.UseMySql(connectionString);

      return new SweetSavoryTreatsContext(builder.Options);
    }
  }
}