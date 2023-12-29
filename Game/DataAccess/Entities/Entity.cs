using System.ComponentModel.DataAnnotations.Schema;

namespace Navislamia.Game.DataAccess.Entities;

public abstract class Entity : TimestampedEntity, ISoftDeletableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

}