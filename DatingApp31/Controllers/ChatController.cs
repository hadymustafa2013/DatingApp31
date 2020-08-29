﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp31.Dtos;
using DatingApp31.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace DatingApp31.Controllers
{
    [Route("api/chat")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [Route("send")]                                           //path looks like this: https://localhost:44379/api/chat/send
        [HttpPost]
        public IActionResult SendRequest([FromBody] ChatMessageDto msg)
        {
            _hubContext.Clients.All.SendAsync("ReceiveOne", msg.user, msg.msgText);
            return Ok();
        }
    }
}
