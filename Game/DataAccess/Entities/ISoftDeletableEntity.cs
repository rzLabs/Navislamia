using System;

namespace Navislamia.Game.DataAccess.Entities;

public interface ISoftDeletableEntity : IEntity
{
    DateTime? DeletedOn { get; set; }
}