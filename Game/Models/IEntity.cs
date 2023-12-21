using System;

namespace Navislamia.Game.Models;

public interface IEntity
{
    long Id { get; set; }
    DateTime? ModifiedOn { get; set; }
}