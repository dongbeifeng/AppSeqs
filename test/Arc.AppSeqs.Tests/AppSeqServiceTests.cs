using NHibernate;
using NSubstitute;
using Xunit;
using static NSubstitute.Substitute;

namespace Arc.AppSeqs.Tests;

public class AppSeqServiceTests
{
    [Fact]
    public async Task GetNextAsyncTest()
    {
        AppSeq a = new AppSeq("a");
        AppSeq b = new AppSeq("b");
        ISession session = For<ISession>();
        session.GetAsync<AppSeq>("a", LockMode.Upgrade).Returns(a);
        session.GetAsync<AppSeq>("b", LockMode.Upgrade).Returns(b);
        ISessionFactory sessionFactory = For<ISessionFactory>();
        sessionFactory.OpenSession().Returns(session);

        AppSeqService sut = new AppSeqService(sessionFactory);
        Assert.Equal(1, await sut.GetNextAsync("a"));
        Assert.Equal(1, await sut.GetNextAsync("b"));

        Assert.Equal(2, await sut.GetNextAsync("a"));
        Assert.Equal(2, await sut.GetNextAsync("b"));
        
        Assert.Equal(3, await sut.GetNextAsync("a"));
        Assert.Equal(3, await sut.GetNextAsync("b"));
        
        Assert.Equal(4, await sut.GetNextAsync("b"));
        Assert.Equal(5, await sut.GetNextAsync("b"));

    }
}