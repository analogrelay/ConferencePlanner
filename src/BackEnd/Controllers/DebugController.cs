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
        public IActionResult ResetDatabase()
        {
            if(_hostingEnvironment.IsDevelopment())
            {
                NDCOsloData.Seed(_applicationDbContext);
                return Accepted();
            }
            else
            {
                return Forbid();
            }
        } 
    }
}
