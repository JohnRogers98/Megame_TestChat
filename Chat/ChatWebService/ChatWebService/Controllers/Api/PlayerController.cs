using ChatWebService.Models.Requests;
using ChatWebService.Services.Interfaces;
using ChatWebService.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebService.Controllers.Api
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        #region Fields

        private readonly IMicrosoftIdentityService _identityService;
        private readonly IPlayerService _playerService; 

        #endregion

        public PlayerController(IPlayerService playerService, IMicrosoftIdentityService identityService)
        {
            _identityService = identityService;
            _playerService = playerService;
        }

        #region Actions

        /// <summary>
        /// Registration of an user
        /// </summary>
        /// <param name="registerRequest">
        /// Request body :{email, password, nickname }
        /// </param>
        /// <returns>200 {} if all right , 400 {}error</returns>
        /// 
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] PlayerRegister registerRequest)
        {
            var result = await _identityService.CreateUserAsync(registerRequest.Email, registerRequest.Password, registerRequest.Nickname, Roles.Player);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Registration of an admin
        /// </summary>
        /// <param name="loginRequest">
        /// Request body :{email, password}
        /// </param>
        /// <returns>200 {token, id} if all right , 400 {}error</returns>
        /// 
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody] PlayerLogin loginRequest)
        {
            var loginResponse = await _identityService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);

            if (loginResponse != null)
            {
                return Ok(loginResponse);
            }
            return BadRequest();
        }

        #endregion

    }
}
