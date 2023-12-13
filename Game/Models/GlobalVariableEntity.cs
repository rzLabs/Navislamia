namespace Navislamia.Game.Models
{
    // this entity is outside Arcadia and Telecaster directory because it is used by both contexts
    public class GlobalVariableEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}