using BoiseState.Tools.Common;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PrejittedLambda.Features.HealthCheck
{
    public class DoHealthCheck
    {
        public class Query : IRequest<Result>
        {

        }

        public class Result
        {
            public DateTime CurrentTime { get; internal set; }
            public Model Status { get; internal set; }
        }

        public class Model
        {
            public bool IsHealthy { get; internal set; }
            public string Reason { get; internal set; } = string.Empty;
        }

        public class QueryHandler : IRequestHandler<Query, Result>
        {
            public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
            {
                return new Result
                {
                    CurrentTime = MountainDateTime.Now,
                    Status = await RunHealthCheck(() =>
                    {
                        return Task.CompletedTask;
                    })
                };
            }
            public async Task<Model> RunHealthCheck(Func<Task> healthCheck)
            {
                var result = new Model();

                try
                {
                    await healthCheck();
                    result.IsHealthy = true;
                }
                catch (Exception e)
                {
                    result.IsHealthy = false;
                    do
                    {
                        result.Reason += e.Message;
                        result.Reason += Environment.NewLine;
                        e = e.InnerException;
                    } while (e != null);
                }

                return result;
            }
        }        
    }
}
