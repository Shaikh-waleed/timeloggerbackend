using TimeLogger.Business.IService;
using TimeLogger.Business.Model;
using TimeLogger.Infrastructure.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TimeLogger.RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        [Produces("application/json", Type = typeof(ResponseModel<List<UserResponseModel>>))]
        public async Task<IActionResult> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var usersModel = await _userService.GetUsers(searchText, page, pageSize);

            return Ok(usersModel);
        }


        [HttpGet("{id}")]
        [Authorize]
        [Produces("application/json", Type = typeof(UserResponseModel))]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _userService.Get(id);

            return Ok(user);
        }
    }
}
