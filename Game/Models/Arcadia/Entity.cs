using System.ComponentModel.DataAnnotations.Schema;

namespace Navislamia.Game.Models.Arcadia
{
    public class Entity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
    
        // TODO SoftDeletable (DeltedOn) + ModfiedOn + CreatedOn
    }
}