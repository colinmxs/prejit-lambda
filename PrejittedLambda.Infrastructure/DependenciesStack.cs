namespace PrejittedLambda.Infrastructure
{
    using Amazon.CDK;
    using Amazon.CDK.AWS.S3;
    using System;

    public class DependenciesStack : Stack
    {
        public Bucket Bucket { get; }
        public DependenciesStack(Construct scope, string id, IStackProps props) : base(scope, id, props)
        {
            Bucket = new Bucket(this, "PrejittedLambda.DependenciesBucket", new BucketProps
            {
                BucketName = "prejittedlambdadependencies290387498273498"
            });
        }
    }
}
