using Api.Domain;
using Api.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UsersController(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }
        
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return Ok(user);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] User user)
        {
            var result = await _userRepository.AddAsync(user);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAsync([FromBody] User user)
        {
            await _userRepository.UpdateAsync(user);
            return Ok(user);
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            
            if (user is null)
            {
                return NotFound();
            }
            
            await _userRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
