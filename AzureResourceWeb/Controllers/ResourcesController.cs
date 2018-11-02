using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AzureResourceWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourcesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<AzureResourceCommon.Dtos.Resources>> Get()
        {
            AzureResourceCommon.Services.ResourceRepository repo = new AzureResourceCommon.Services.ResourceRepository();
            AzureResourceCommon.Dtos.Resources[] resources = repo.GetResources();

            return resources;
        }
    }
}
