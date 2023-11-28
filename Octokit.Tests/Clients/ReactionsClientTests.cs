using System;
using Xunit;

namespace Octokit.Tests.Clients
{
    public class ReactionsClientTests
    {
        public class TheCtor
        {
            [Fact]
            public void EnsuresNonNullArguments()
            {
                Assert.Throws<ArgumentNullException>(() => new ReactionsClient(null));
            }
        }
    }
}
