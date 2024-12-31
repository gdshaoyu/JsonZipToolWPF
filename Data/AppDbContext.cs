using Microsoft.EntityFrameworkCore;
using JsonZipToolWPF.Models;
using System.IO;

namespace JsonZipToolWPF.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<ConversionRecord> ConversionRecords { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JsonZipTool",
                "conversion_history.db");

            // 确保目录存在
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }
} 