using DevConsole.Extensions;
using Navislamia.Configuration.Options;
using Npgsql;

namespace Tests.DataAccess;

[TestFixture]
public class DatabaseOptionsTests
{

    [SetUp]
    public void Setup()
    {
        
    }
    
    [Test]
    public void DatabaseOptions_ReturnsConnectionString_WhenOptionsAreValid()
    {
        var databaseOptions = new DatabaseOptions
        {
            DataSource = "somewhere",
            Port = 5432,
            User = "postgres",
            Password = "t0p$eCr3t!",
            InitialCatalog = "myDatabase",
            IncludeErrorDetail = true
        };

        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseOptions.DataSource,
            Database = databaseOptions.InitialCatalog,
            Port = databaseOptions.Port,
            Username = databaseOptions.User,
            Password = databaseOptions.Password,
            SslMode = SslMode.Prefer,
            MaxPoolSize = 5,
            CommandTimeout = 30,
            IncludeErrorDetail = databaseOptions.IncludeErrorDetail
        };

        var expected = connectionStringBuilder.ConnectionString;

        Assert.That(expected, Is.EqualTo(databaseOptions.ConnectionString()));
    }

    [Test]
    public void DatabaseOptions_Throws_WhenNoOptionsSet()
    {
        var options = new DatabaseOptions();
        Assert.Throws<ArgumentOutOfRangeException>(() => options.ConnectionString());
    }
}