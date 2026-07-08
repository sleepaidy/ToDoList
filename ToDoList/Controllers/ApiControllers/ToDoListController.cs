using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Interfaces;

namespace ToDoList.Controllers.ApiControllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class ToDoListController : ControllerBase
    {
        private readonly IToDoListService _toDoListService;
        private readonly IAuthService _authService;

        public ToDoListController(IToDoListService toDoListService, IAuthService authService)
        {
            _toDoListService = toDoListService;
            _authService = authService;
        }

        [HttpPost]
        public IActionResult MoveUp(int id)
        {
            var userId = _authService.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            return _toDoListService.MoveTaskUp(id, userId)
                ? Ok(new { success = true })
                : BadRequest();
        }

        [HttpPost]
        public IActionResult MoveDown(int id)
        {
            var userId = _authService.GetUserId();
            if (userId <= 0)
            {
                return Unauthorized();
            }

            return _toDoListService.MoveTaskDown(id, userId)
                ? Ok(new { success = true })
                : BadRequest();
        }
    }
}
