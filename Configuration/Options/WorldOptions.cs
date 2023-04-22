using System.ComponentModel.DataAnnotations;
using Configuration.Options;

namespace Navislamia.Configuration.Options
{
    public abstract class WorldOptions : IDbCredentials
    {
        [Required] 
        public string Ip { get; set; }
        
        [Required]
        public string Port { get; set; }
        
        [Required]
        public string DbName { get; set; }
        
        [Required]
        public string User { get; set; }
        
        [Required]
        public string Password { get; set; }

        public bool IsTrustedConnection { get; set; }
    }
}