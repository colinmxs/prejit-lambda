namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;

    sealed class Program
    {
        public static void Main(string[] args)
        {
            var settings = Utilities.LoadSettingsFromJsonFile("appsettings.json");
            var tags = Utilities.LoadSettingsFromJsonFile("tags.json");
            var environment = tags["Environment"];
            var app = new App();
            _ = new MainStack(app, $"{"PrejittedLambda".Replace('.', '-')}-{environment}", new StackProps
            {
                Tags = tags,
                Env = new Environment
                {
                    Region = "us-west-2",
                    Account = settings["AccountNumber"]
                }
            });
            app.Synth();
        }        
    }
}
