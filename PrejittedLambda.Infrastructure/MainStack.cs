namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.APIGateway;
    using PrejittedLambda.Infrastructure.Constructs;

    public class MainStack : Stack
    {
        public class MainStackProps : StackProps
        {
            public string LayerArn { get; internal set; }
            public string StorePath { get; internal set; }
        }

        public MainStack(Construct scope, string id, MainStackProps props) : base(scope, id, props)
        {
            var apiGatewayProxyLambdaProps = new ApiGatewayProxyLambdaProps
            {
                ConstructIdPrefix = "PrejittedLambda",
                ApplicationId = "PrejittedLambda",
                LambdaExecutionRoleName = "PrejittedLambdaRole",
                LambdaFunctionHandler = "PrejittedLambda::PrejittedLambda.LambdaEntryPoint::FunctionHandlerAsync",
                LambdaFunctionName = "PrejittedLambdaFunction",
                LambdFunctionAssetCodePath = $"{Utilities.GetDirectory("PrejittedLambda")}\\publish.zip",
                StorePath = props.StorePath,
                RestApiName = $"PrejittedLambda",
                AspNetEnvironment = "Production",
                CorsOptions = new CorsOptions
                {
                    AllowCredentials = true,
                    /// TODO: #2 Set up CORS!
                    AllowOrigins = new string[] { "" },
                    AllowMethods = Cors.ALL_METHODS
                },
                LayerArn = props.LayerArn
            };

            var apiGatewayProxyLambda = new ApiGatewayProxyLambda(this, apiGatewayProxyLambdaProps.ConstructIdPrefix, apiGatewayProxyLambdaProps);

            //var dependenciesBucket = new Bucket(this, "PrejittedLambda.DependenciesBucket", new BucketProps 
            //{
                
            //});
        }
    }
}
