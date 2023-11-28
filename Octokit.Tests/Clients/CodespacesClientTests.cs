using System;
using NSubstitute;
using Octokit;
using Xunit;

public class CodespacesClientTests
{
    public class TheCtor
    {
        [Fact]
        public void EnsuresNonNullArguments()
        {
            Assert.Throws<ArgumentNullException>(() => new CodespacesClient(null));
        }
    }

    public class TheGetAllMethod
    {
        [Fact]
        public void RequestsCorrectGetAllUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new CodespacesClient(connection);

            client.GetAll();
            connection.Received().Get<CodespacesCollection>(Arg.Is<Uri>(u => u.ToString() == "user/codespaces"));
        }

        [Fact]
        public void RequestsCorrectGetForRepositoryUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new CodespacesClient(connection);
            client.GetForRepository("owner", "repo");
            connection.Received().Get<CodespacesCollection>(Arg.Is<Uri>(u => u.ToString() == "repos/owner/repo/codespaces"));
        }

        [Fact]
        public void RequestsCorrectGetUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new CodespacesClient(connection);
            client.Get("codespaceName");
            connection.Received().Get<Codespace>(Arg.Is<Uri>(u => u.ToString() == "user/codespaces/codespaceName"));
        }

        [Fact]
        public void RequestsCorrectStartUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new CodespacesClient(connection);
            client.Start("codespaceName");
            connection.Received().Post<Codespace>(Arg.Is<Uri>(u => u.ToString() == "user/codespaces/codespaceName/start"));
        }

        [Fact]
        public void RequestsCorrectStopUrl()
        {
            var connection = Substitute.For<IApiConnection>();
            var client = new CodespacesClient(connection);
            client.Stop("codespaceName");
            connection.Received().Post<Codespace>(Arg.Is<Uri>(u => u.ToString() == "user/codespaces/codespaceName/stop"));
        }
    }
}
