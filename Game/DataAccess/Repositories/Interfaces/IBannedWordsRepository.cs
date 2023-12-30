namespace Navislamia.Game.DataAccess.Repositories.Interfaces;

public interface IBannedWordsRepository
{
    /// <summary>
    /// Check if value equals a word in BannedResource
    /// </summary>
    /// <param name="value"></param>
    bool IsBannedWord(string value);
    
    /// <summary>
    /// Check if value is a substring of a banned word
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    bool ContainsBannedWord(string value);
}