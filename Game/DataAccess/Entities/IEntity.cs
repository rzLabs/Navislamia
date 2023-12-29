using System;

namespace Navislamia.Game.DataAccess.Entities;

public interface IEntity
{
    long Id { get; set; }
    DateTime? ModifiedOn { get; set; }
}