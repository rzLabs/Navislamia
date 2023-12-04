using System.ComponentModel.DataAnnotations.Schema;

namespace DevConsole.Models.Arcadia;

public class Entity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    
    // TODO SoftDeletable (DeltedOn) + ModfiedOn + CreatedOn
}