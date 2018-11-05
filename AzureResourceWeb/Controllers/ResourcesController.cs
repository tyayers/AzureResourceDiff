using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureResourceCommon.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace AzureResourceWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResourceController : ControllerBase
    {
        // GET api/resources
        [HttpGet]
        public ActionResult<IEnumerable<AzureResourceCommon.Dtos.Resource>> Get()
        {
            AzureResourceCommon.Services.ResourceRepository repo = new AzureResourceCommon.Services.ResourceRepository();
            AzureResourceCommon.Dtos.Resource[] resources = repo.GetResources(20);

            return resources;
        }

        // GET api/getchanges
        [HttpGet("{resourceid}")]
        public ActionResult<Resource> Get(int resourceid)
        {
            Resource result = null;
            AzureResourceCommon.Services.ResourceRepository repo = new AzureResourceCommon.Services.ResourceRepository();

            result = repo.GetResource(resourceid);
            return result;
        }

        // GET api/getchanges
        [HttpGet]
        [Route("changes")]
        public ActionResult<IEnumerable<DailyResourceChanges>> GetChanges()
        {
            List<DailyResourceChanges> changes = new List<DailyResourceChanges>();
            AzureResourceCommon.Services.ResourceRepository repo = new AzureResourceCommon.Services.ResourceRepository();
            AzureResourceCommon.Dtos.Resource[] resources = repo.GetResources(20);

            foreach (AzureResourceCommon.Dtos.Resource rec in resources)
            {
                if (!String.IsNullOrEmpty(rec.Differences))
                {
                    DailyResourceChanges dailyChanges = new DailyResourceChanges();
                    dailyChanges.Id = rec.Id;
                    dailyChanges.Timestamp = rec.Timestamp;
                    // We have changes
                    // First parse resources (big)
                    JObject recJson = JObject.Parse(rec.ResourcesJson);

                    Newtonsoft.Json.Linq.JObject diffJson = Newtonsoft.Json.Linq.JObject.Parse(rec.Differences);

                    int value;
                    foreach (JProperty prop in diffJson["value"])
                    {
                        if (int.TryParse(prop.Name, out value))
                        {
                            // We have a change
                            ResourceChanges change = new ResourceChanges();
                            JObject recDetailJson = (JObject)recJson["value"][value];
                            change.Namespace = recDetailJson["namespace"].ToString();
                            change.Differences = prop.Value.ToString();

                            dailyChanges.Changes.Add(change);
                        }
                        else if (prop.Name == "_t")
                        {
                            if (prop.Value.ToString() == "a")
                                dailyChanges.Additions = diffJson["value"].Children<JToken>().ToList<JToken>().Count - 1;
                            else
                                dailyChanges.Others = diffJson["value"].Children<JToken>().ToList<JToken>().Count - 1;
                        }
                    }

                    changes.Add(dailyChanges);
                }
            }

            return changes;
        }
    }
}
