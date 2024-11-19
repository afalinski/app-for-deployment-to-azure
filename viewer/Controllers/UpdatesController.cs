using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using viewer.Hubs;
using viewer.Models;
using System.Reflection.Metadata;
using Microsoft.Extensions.Logging;

namespace viewer.Controllers
{
    [Route("api/[controller]")]
    public class UpdatesController : Controller
    {
        #region Data Members
        private readonly ILogger<UpdatesController> _logger;

        private bool EventTypeNotification
            => HttpContext.Request.Headers["aeg-event-type"].FirstOrDefault() ==
               "Notification";

        private readonly IHubContext<GridEventsHub> _hubContext;

        #endregion

        #region Constructors

        public UpdatesController(IHubContext<GridEventsHub> gridEventsHubContext, ILogger<UpdatesController> logger)
        {
            this._hubContext = gridEventsHubContext;
            _logger = logger;
        }

        #endregion

        #region Public Methods

        [HttpOptions]
        public async Task<IActionResult> Options()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var webhookRequestOrigin = HttpContext.Request.Headers["WebHook-Request-Origin"].FirstOrDefault();
                var webhookRequestCallback = HttpContext.Request.Headers["WebHook-Request-Callback"];
                var webhookRequestRate = HttpContext.Request.Headers["WebHook-Request-Rate"];
                HttpContext.Response.Headers.Add("WebHook-Allowed-Rate", "*");
                HttpContext.Response.Headers.Add("WebHook-Allowed-Origin", webhookRequestOrigin);
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);

            var jsonContent = await reader.ReadToEndAsync();


            if (EventTypeNotification)
            {
                return await HandleGridEvents(jsonContent);
            }

            return BadRequest();
        }

        #endregion

        #region Private Methods

        private async Task<IActionResult> HandleGridEvents(string jsonContent)
        {
            try
            {
                await this._hubContext.Clients.All.SendAsync(
                    "gridupdate",
                    Guid.NewGuid(),
                    string.Empty,
                    string.Empty,
                    DateTime.Now.ToShortDateString(),
                    jsonContent);

                return Ok();
            }
            catch (Exception e)
            {
               _logger.LogError(e.Message);
               throw;
            }
        }

        #endregion
    }
}