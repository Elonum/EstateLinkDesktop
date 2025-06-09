using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EstateLinkAvalonia.Data
{
    public static class DatabaseService
    {
        private static IConfiguration? _configuration;
        private static EstateLinkContext? _context;

        public static void Initialize()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<EstateLinkContext>();
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));

            _context = new EstateLinkContext(optionsBuilder.Options);
        }

        public static EstateLinkContext GetContext()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Database context is not initialized. Call Initialize() first.");
            }
            return _context;
        }
    }
} 