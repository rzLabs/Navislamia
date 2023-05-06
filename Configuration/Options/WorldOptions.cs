using System.ComponentModel.DataAnnotations;

namespace Navislamia.Configuration.Options
{
    public class WorldOptions : IDbCredentials
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