using System;

namespace Navislamia.Game.Models;

public abstract class TimestampedEntity
{
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime? ModifiedOn { get; set; }

    public DateTime? DeletedOn { get; set; }
}