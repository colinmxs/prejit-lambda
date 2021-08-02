namespace PrejittedLambda.Infrastructure.Constructs
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.APIGateway;
    using Amazon.CDK.AWS.IAM;
    using Amazon.CDK.AWS.Lambda;
    using Amazon.CDK.AWS.S3;
    using System.Collections.Generic;

    public class ApiGatewayProxyLambdaProps
    {
        /// <summary>
        /// The prefix to append to logical IDs of resources in this construct. This value should be unique amongst ApiGatewayProxyLambda construct instances
        /// ex: "MyBackendServiceAuthApi"
        /// </summary>
        public string ConstructIdPrefix { get; set; }

        /// <summary>
        /// The unique identifier of the application or service. Used for tagging
        /// ex: "MyBackendService"
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        /// The name to assign to the generated IAM Role
        /// ex: "MyBackendServiceAuthApiRole"
        /// </summary>
        public string LambdaExecutionRoleName { get; set; }

        /// <summary>
        /// The name to assign to the generated Lambda Function
        /// ex: "MyBackendServiceAuthApiFunction"
        /// </summary>
        public string LambdaFunctionName { get; set; }

        /// <summary>
        /// The value to assign to the Handler property of the generated lambda function
        /// ex: "MyBackendService.AuthApi::MyBackendService.AuthApi.LambdaEntryPoint::FunctionHandlerAsync"
        /// </summary>
        public string LambdaFunctionHandler { get; set; }

        /// <summary>
        /// The local path of the lambda deployment package
        /// ex: $"{Directory.GetCurrentDirectory()}\\LambdaPackage\\MyBackendService.AuthApi.zip"
        /// </summary>
        public string LambdFunctionAssetCodePath { get; set; }

        /// <summary>
        /// The name to assign to the generated Lambda Rest Api
        /// ex: MyBackendServiceAuthApiRestApi
        /// </summary>
        public string RestApiName { get; set; }
        /// <summary>
        /// The value to assign to ASPNETCORE_ENVIRONMENT environment variable on the lambda function
        /// </summary>
        public string AspNetEnvironment { get; set; }
        /// <summary>
        /// The CorsOptions to add to the ApiGateway
        /// </summary>
        public ICorsOptions CorsOptions { get; set; }
        /// <summary>
        /// The number of provisioned (warm) lambda instances in the production environment
        /// </summary>
        public int ProvisionedProductionInstances { get; set; } = 0;    
        public Bucket LayerBucket { get; set; }
        public string LayerKey { get; set; }
    }

    public class ApiGatewayProxyLambda : Construct
    {
        internal Role LambdaExecutionRole { get; }
        internal Function LambdaFunction { get; }
        internal LambdaRestApi RestApi { get; }

        public ApiGatewayProxyLambda(Construct scope, string id, ApiGatewayProxyLambdaProps props) : base(scope, id)
        {
            Tags.Of(this).Add(nameof(props.ApplicationId), props.ApplicationId);

            LambdaExecutionRole = new Role(this, "LambdaExecutionRole", new RoleProps
            {
                AssumedBy = new ServicePrincipal("lambda.amazonaws.com"),
                RoleName = props.LambdaExecutionRoleName,
                InlinePolicies = new Dictionary<string, PolicyDocument>
                {
                    {
                        "cloudwatch-policy",
                        new PolicyDocument(
                            new PolicyDocumentProps {
                                AssignSids = true,
                                Statements = new [] {
                                    new PolicyStatement(new PolicyStatementProps {
                                        Effect = Effect.ALLOW,
                                        Actions = new string[] {
                                            "logs:CreateLogStream",
                                            "logs:PutLogEvents",
                                            "logs:CreateLogGroup"
                                        },
                                        Resources = new string[] {
                                            "arn:aws:logs:*:*:*"
                                        }
                                    })
                                }
                            })
                    }
                }
            });
            Tags.Of(LambdaExecutionRole).Add("Name", props.LambdaExecutionRoleName);
            Tags.Of(LambdaExecutionRole).Add("ApplicationRole", "Lambda Excution Role");

            LambdaFunction = new Function(this, "LambdaFunction", new FunctionProps
            {
                Code = new AssetCode(props.LambdFunctionAssetCodePath),
                Handler = props.LambdaFunctionHandler,
                Runtime = Runtime.DOTNET_CORE_3_1,
                Timeout = Duration.Seconds(30),
                FunctionName = props.LambdaFunctionName,
                CurrentVersionOptions = new VersionOptions
                {
                    ProvisionedConcurrentExecutions = props.AspNetEnvironment == "Production" ? props.ProvisionedProductionInstances : 0
                },
                Layers = new LayerVersion[] 
                {
                    new LayerVersion(this, "LambdaLayer", new LayerVersionProps
                    {
                        Code = Code.FromBucket(props.LayerBucket, props.LayerKey)
                    })
                },
                MemorySize = 256,
                RetryAttempts = 1,
                Role = LambdaExecutionRole,
                Environment = new Dictionary<string, string>
                {
                    {"ASPNETCORE_ENVIRONMENT", props.AspNetEnvironment}
                }
            });
            Tags.Of(LambdaFunction).Add("Name", props.LambdaFunctionName);
            Tags.Of(LambdaFunction).Add("ApplicationRole", "Lambda Function");
                        
            RestApi = new LambdaRestApi(this, "RestApi", new LambdaRestApiProps
            {
                Handler = LambdaFunction,
                Proxy = true,
                Deploy = true,
                DefaultCorsPreflightOptions = props.CorsOptions,
                DeployOptions = new StageOptions
                {
                    StageName = "v1"
                },
                RestApiName = props.RestApiName,
                EndpointConfiguration = new EndpointConfiguration
                {
                    Types = new EndpointType[]
                    {
                        EndpointType.REGIONAL
                    }
                }
            });
            Tags.Of(RestApi).Add("Name", props.RestApiName);
            Tags.Of(RestApi).Add("ApplicationRole", "API Gateway");
        }
    }
}
