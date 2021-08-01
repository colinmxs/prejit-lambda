namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;

    sealed class Program
    {
        public static void Main(string[] args)
        {
            var app = new App();
            _ = new MainStack(app, "PrejittedLambda", new StackProps());
            app.Synth();
        }        
    }
}
