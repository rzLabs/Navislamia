using System.ComponentModel.DataAnnotations;

namespace Navislamia.Configuration.Options
{
    public class DatabaseOptions
    {
        [Required]
        public string DataSource { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string User { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string InitialCatalog { get; set; }

        public int? MaxPoolSize { get; set; }

        public int CommandTimeout { get; set; } = 30;

        public int CommandTimeoutMigration { get; set; } = 3600;
    }
}