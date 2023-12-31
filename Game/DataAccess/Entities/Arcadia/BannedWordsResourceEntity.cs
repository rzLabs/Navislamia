
namespace Navislamia.Game.DataAccess.Entities.Arcadia;

// To support multiple languages we could either duplicate this table or include a enum with locale information
public class BannedWordsResourceEntity
{
    public long Id { get; set; }
    public string Word { get; set; }
}