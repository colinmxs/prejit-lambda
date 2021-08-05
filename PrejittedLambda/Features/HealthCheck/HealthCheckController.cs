namespace PrejittedLambda.Features.HealthCheck
{
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        //private readonly IMediator _mediator;

        //public HealthCheckController(IMediator mediator)
        //{
        //    _mediator = mediator;
        //}

        [HttpGet]
        [Route("")]
        [AllowAnonymous]
        public async Task<DoHealthCheck.Result> DoHealthCheck()
        {
            return new DoHealthCheck.Result
            {
                CurrentTime = System.DateTime.Now,
                Status = new DoHealthCheck.Model
                {
                    IsHealthy = true,
                    Reason = "200"
                }
            };
            //return await _mediator.Send(new DoHealthCheck.Query());
        }
    }
}
