namespace MigrateDatabase;

public class MigrationToolOptions
{
    public string Ip { get; set; }
    public int Port { get; set; }
    public bool IsTrustedConnection { get; set; }
    public string DbName { get; set; }
    public string User { get; set; }
    public string Password { get; set; }
}