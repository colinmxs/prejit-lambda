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
                    Account = app.Account,
                    Region = "us-west-2"
                }
            });

            _ = new MainStack(app, "PrejittedLambda", new MainStack.MainStackProps 
            {
                LayerBucket = dependenciesStack.Bucket,
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
