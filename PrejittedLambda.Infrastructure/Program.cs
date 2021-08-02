namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;

    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            var dependenciesStack = new DependenciesStack(app, "Dependencies", new StackProps
            {
                Env = new Environment
                {
                    Account = "685696558467",
                    Region = "us-west-2"
                }
            });

            _ = new MainStack(app, "PrejittedLambda", new StackProps 
            {
                Env = new Environment
                {
                    Account = "685696558467",
                    Region = "us-west-2"
                }
            });
            app.Synth();
        }        
    }
}
