namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;
    using System.Threading.Tasks;

    sealed class Program
    {
        public static async Task Main(string[] args)
        {
            var layerArn = (await Utilities.LoadFromJsonFile("appsettings.json"))["LayerArn"];
            var app = new App();
            var dependenciesStack = new DependenciesStack(app, "Dependencies", new StackProps
            {
                Env = new Environment
                {
                    Account = app.Account,
                    Region = "us-west-2"
                }
            });

            _ = new MainStack(app, "MainStack", new MainStack.MainStackProps
            {
                LayerArn = layerArn,
                Env = new Environment
                {
                    Account = app.Account,
                    Region = "us-west-2"
                },

            });
            app.Synth();
        }        
    }
}
