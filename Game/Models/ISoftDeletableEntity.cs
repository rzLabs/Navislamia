using System;

namespace Navislamia.Game.Models;

public interface ISoftDeletableEntity : IEntity
{
    DateTime? DeletedOn { get; set; }
}