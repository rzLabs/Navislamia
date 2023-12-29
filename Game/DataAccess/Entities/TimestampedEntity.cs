using System;

namespace Navislamia.Game.DataAccess.Entities;

public abstract class TimestampedEntity
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedOn { get; set; }

    public DateTime? DeletedOn { get; set; }
}