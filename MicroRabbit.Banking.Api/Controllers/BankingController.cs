using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MicroRabbit.Banking.Application.Interfaces;
using MicroRabbit.Banking.Application.Models;
using MicroRabbit.Banking.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MicroRabbit.Banking.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankingController : ControllerBase
    {
        private readonly IAccountService _service;

        public BankingController(IAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Account>> Get()
        {
            return Ok(_service.GetAccounts());
        }

        [HttpPost]
        public IActionResult Post([FromBody] AccountTransafer transafer)
        {
            _service.Transfer(transafer);
            return Ok(transafer);
        }
    }
}
