using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatWebService.Controllers.Api
{
    [ApiController]
    [Route("[controller]")]
    public class RolesController : ControllerBase
    {
        #region Fields

        private readonly RoleManager<IdentityRole> _roleManager;

        #endregion

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        #region Actions

        /// <summary>
        /// Create new role
        /// </summary>
        /// <param name="name">
        /// Query body :{String} - role name
        /// </param>
        /// <returns>200 {} - if all right , 400{} - error</returns>
        /// 
        [HttpPost("create")]
        public async Task<IActionResult> CreateRole([FromQuery]String name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return Ok();
                }
            }
            return BadRequest();
        }

        #endregion

    }
}
