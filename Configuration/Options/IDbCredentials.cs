using System.ComponentModel.DataAnnotations;

namespace Navislamia.Configuration.Options
{
    public interface IDbCredentials
    {
        [Required] 
        protected string Ip { get; set; }
        
        [Required]
        protected string Port { get; set; }
        
        [Required]
        protected string DbName { get; set; }
        
        [Required]
        protected string User { get; set; }
        
        [Required]
        protected string Password { get; set; }

        protected bool IsTrustedConnection { get; set; }
        
    }
}