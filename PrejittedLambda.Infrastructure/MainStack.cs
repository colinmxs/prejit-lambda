namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.APIGateway;
    using PrejittedLambda.Infrastructure.Constructs;
    using System;
    using System.IO;

    public class MainStack : Stack
    {
        public MainStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
        {
            // TODO: #1 Check generated prop values
            var settings = Utilities.LoadSettingsFromJsonFile("appsettings.json");
            var apiGatewayProxyLambdaProps = new ApiGatewayProxyLambdaProps
            {
                ConstructIdPrefix = "PrejittedLambda",
                ApplicationId = "PrejittedLambda",
                LambdaExecutionRoleName = $"{("PrejittedLambda.Role").Replace(".", string.Empty)}-{props.Tags["Environment"]}",
                LambdaFunctionHandler = "PrejittedLambda::PrejittedLambda.LambdaEntryPoint::FunctionHandlerAsync",
                LambdaFunctionName = $"{("PrejittedLambda.Function").Replace(".", string.Empty)}-{props.Tags["Environment"]}",
                LambdFunctionAssetCodePath = $"{Utilities.GetDirectory("PrejittedLambda")}\\publish.zip",
                RestApiName = $"PrejittedLambda-{props.Tags["Environment"]}",
                AspNetEnvironment = props.Tags["Environment"],
                CorsOptions = new CorsOptions
                {
                    AllowCredentials = true,
                    /// TODO: #2 Set up CORS!
                    AllowOrigins = new string[] { "" },
                    AllowMethods = Cors.ALL_METHODS
                },
                SubDomain = "",
                SSLCertId = settings["SSLCertId"],                
                Domain = settings["Domain"],
                AccountNumber = settings["AccountNumber"]
            };

            var apiGatewayProxyLambda = new ApiGatewayProxyLambda(this, apiGatewayProxyLambdaProps.ConstructIdPrefix, apiGatewayProxyLambdaProps);
        }
    }
}
