using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FunctionProj;

public class httpfun
{
    private readonly ILogger<httpfun> _logger;

    public httpfun(ILogger<httpfun> logger)
    {
        _logger = logger;
    }

    [Function("httpfun")]
    public IActionResult RunA(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route ="fun/{txt}")] HttpRequest req,
        string txt)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult($"Welcome to Azure Functions! ({txt})");
    }
}

