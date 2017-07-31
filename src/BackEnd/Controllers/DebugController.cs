using System.Threading.Tasks;
using BackEnd.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BackEnd.Controllers
{
    [Route("api/[controller]")]
    public class DebugController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DebugController(ApplicationDbContext applicationDbContext, IHostingEnvironment hostingEnvironment)
        {
            _applicationDbContext = applicationDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("db/reset")]
        public async Task<IActionResult> ResetDatabase()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                await NDCOsloData.Recreate(_applicationDbContext);
                return Accepted();
            }
            else
            {
                return Forbid();
            }
        }

        [HttpPost]
        [Route("db/seed")]
        public async Task<IActionResult> SeedDatabase()
        {
            if (_hostingEnvironment.IsDevelopment())
            {
                await NDCOsloData.Clear(_applicationDbContext);
                await NDCOsloData.Seed(_applicationDbContext);
                return Accepted();
            }
            else
            {
                return Forbid();
            }
        }
    }
}
