using FakeItEasy;
using FluentAssertions;
using Navislamia.Game.Network.Entities;
using Navislamia.Game.Network.Entities.Actions;
using Navislamia.Game.Network.Packets.Enums;

namespace Tests.Network;

[TestFixture]
public class AuthTests 
{
    private readonly AuthActions _authActions;
    private List<GameClient> GameClients { get; set; } = new();
    
    private readonly AuthClient _sut;
    
    public AuthTests(CommonFixture fixture)
    {
        _authActions = new AuthActions(GameClients);

        _sut = new AuthClient("ip", 0, GameClients, _authActions);
    }

    [Test]
    public void CreateClient()
    {
        _sut.Type.Should().Be(ClientType.Auth);
        _sut.Connection.Should().NotBe(null);
        _sut.Connection.Connected.Should().Be(true);
        A.CallTo(() => _sut.Connection.Start()).MustHaveHappened();
    }
    
}
