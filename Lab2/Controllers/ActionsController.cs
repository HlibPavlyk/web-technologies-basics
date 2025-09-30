using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab2.Controllers;

[ApiController]
[Route("api/actions")]
public class ActionsController : ControllerBase
{
    [HttpGet("admin-report")]
    [Authorize(Roles = "Administrator")]
    public Task<string> GetAdminReport() 
        => Task.FromResult("Only administrators are allowed to generate a detailed administrative report.");

    [HttpGet("report")]
    [Authorize(Roles = "Administrator, User")]
    public Task<string> GetUserReport() 
        => Task.FromResult("Both administrators and regular users are allowed to access this general report.");

    [HttpGet("info")]
    public Task<string> GetInfo() 
        => Task.FromResult("This public information is available to everyone, including unauthorized users.");
}

