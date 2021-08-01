namespace PrejittedLambda
{
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using System.Reflection;
    using System.Threading.Tasks;
    using System;
    using System.IO;
    using FakeItEasy;
    using MediatR;

    internal static class TestFixture
    {
        internal static readonly IConfiguration Configuration;
        private static readonly IServiceScopeFactory ScopeFactory;

        static TestFixture()
        {
            var startupAssembly = typeof(Startup).GetTypeInfo().Assembly;
            var projectPath = GetProjectPath("PrejittedLambda");

            var host = A.Fake<IWebHostEnvironment>();
            A.CallTo(() => host.ContentRootPath).Returns(projectPath);
            A.CallTo(() => host.EnvironmentName).Returns("Development");

            var startup = new Startup(A.Fake<IConfiguration>(), host);
            Configuration = Startup.Configuration;

            var services = new ServiceCollection();
            startup.ConfigureServices(services);

            var provider = services.BuildServiceProvider();
            ScopeFactory = provider.GetService<IServiceScopeFactory>();
        }

        internal static string GetProjectPath(string projectName)
        {
            var projectRelativePath = @"";

            // Get currently executing test project path
            var applicationBasePath = AppContext.BaseDirectory;

            // Find the path to the target project
            var directoryInfo = new DirectoryInfo(applicationBasePath);
            do
            {
                directoryInfo = directoryInfo.Parent;

                var projectDirectoryInfo = new DirectoryInfo(Path.Combine(directoryInfo.FullName, projectRelativePath));
                if (projectDirectoryInfo.Exists)
                {
                    var projectFileInfo = new FileInfo(Path.Combine(projectDirectoryInfo.FullName, projectName, $"{projectName}.csproj"));
                    if (projectFileInfo.Exists)
                    {
                        return Path.Combine(projectDirectoryInfo.FullName, projectName);
                    }
                }
            }
            while (directoryInfo.Parent != null);

            throw new Exception($"Project root could not be located using the application root {applicationBasePath}.");
        }

        internal static int HealthCheck()
        {
            try
            {
                if (Configuration == null || ScopeFactory == null)
                {
                    return 0;
                }
            }
            catch { return 0; }
            return 1;
        }

        public static async Task ExecuteScopeAsync(Func<IServiceProvider, Task> action)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                await action(scope.ServiceProvider);
            }
        }

        public static async Task<T> ExecuteScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
        {
            using (var scope = ScopeFactory.CreateScope())
            {
                try
                {
                    var result = await action(scope.ServiceProvider);
                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public static Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();
                return mediator.Send(request);
            });
        }

        public static Task SendAsync(IRequest request)
        {
            return ExecuteScopeAsync(sp =>
            {
                var mediator = sp.GetService<IMediator>();
                return mediator.Send(request);
            });
        }
    }
}
