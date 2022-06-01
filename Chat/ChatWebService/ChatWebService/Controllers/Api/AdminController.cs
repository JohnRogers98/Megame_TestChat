using ChatWebService.Helpers;
using ChatWebService.Models.Requests;
using ChatWebService.Services.Interfaces;
using ChatWebService.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace ChatWebService.Controllers.Api
{
    [ApiController]
    [Route("[Controller]")]
    [Authorize]
    public class AdminController : ControllerBase
    {
        #region Fields

        private readonly IMicrosoftIdentityService _identityService;
        private readonly IAdminService _adminService;

        private readonly IDistributedCache _distributedCache;

        private String? _userId => User?.Claims.First(x => x.Type == "id").Value;

        #endregion

        public AdminController(IAdminService adminService, IMicrosoftIdentityService identityService, IDistributedCache distributedCache)
        {
            _identityService = identityService;
            _adminService = adminService;
            _distributedCache = distributedCache; 
        }

        #region Actions

        /// <summary>
        /// Registration of an admin
        /// </summary>
        /// <param name="registerRequest">
        /// Request body :{email, password, name }
        /// </param>
        /// <returns>200 {} if all right , 400 {}error</returns>
        /// 
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]AdminRegister registerRequest)
        {
            var result = await _identityService.CreateUserAsync(registerRequest.Email, registerRequest.Password, registerRequest.Name, Roles.Admin);

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
        /// Authentication of an Admin
        /// </summary>
        /// <param name="loginRequest">
        /// Request body :{email, password}
        /// </param>
        /// <returns>200 - {token , id} } if all right , 400 - {} error</returns>
        /// 
        [AllowAnonymous]
        [HttpPost("auth")]
        public async Task<IActionResult> Authenticate([FromBody]AdminLogin loginRequest)
        {
            var loginResponse = await _identityService.AuthenticateAsync(loginRequest.Email, loginRequest.Password);

            if (loginResponse != null) 
            {
                return Ok(loginResponse);
            }
            return BadRequest();
        }

        /// <summary>
        /// Get unread messages count (this method used redis cache)
        /// </summary>
        /// <returns>200 - {Int32} } if all right , 400 - {} error</returns>
        /// 
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("messages/unread-count")]
        public async Task<IActionResult> UnreadMessagesCount()
        {
            //TODO: create Resourses for cache key 
            String cacheUnreadCount_Key = $"unread_{_userId}";
            Int32 unreadCount;

            //TODO: expose setter of cache in database service for more concise result
            try
            {
                var cacheUnreadCountValue = await _distributedCache.GetAsync(cacheUnreadCount_Key);
                if (cacheUnreadCountValue != null)
                {
                    var serializedString = Encoding.UTF8.GetString(cacheUnreadCountValue);
                    unreadCount = JsonConvert.DeserializeObject<Int32>(serializedString);
                }
                else
                {
                    unreadCount = _adminService.GetUnreadMessagesCountByOperatorId(_userId);

                    _distributedCache.SetCache(cacheUnreadCount_Key, unreadCount);
                }
                
                return Ok(unreadCount);
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        #endregion

        #region Private Methods

        #endregion

    }
}