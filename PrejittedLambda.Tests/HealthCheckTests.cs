namespace PrejittedLambda.Tests
{
    using PrejittedLambda.Features.HealthCheck;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Threading.Tasks;
    using static PrejittedLambda.TestFixture;

    [TestClass]
    public class HealthCheckTests
    {
        [TestMethod]
        public async Task SmokeTest()
        {
            var query = new DoHealthCheck.Query();
            await SendAsync(query);
        }
    }
}
