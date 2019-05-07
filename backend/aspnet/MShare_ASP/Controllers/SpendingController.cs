﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MShare_ASP.Data;
using MShare_ASP.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Controllers {
    /// <summary>
    /// Controller is responsible for anything spending related, e.g. adding a new spending to a group, updating, deleting it
    /// </summary>
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class SpendingController : BaseController {
        private ISpendingService SpendingService { get; }
        /// <summary>
        /// Initializes the SpendingController
        /// </summary>
        /// <param name="spendingService"></param>
        public SpendingController(ISpendingService spendingService) {
            SpendingService = spendingService;
        }

        /// <summary>
        /// Gets all the spendings associated with a group
        /// </summary>
        /// <param name="id">id of the group</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<IList<API.Response.SpendingData>>> Get(long id) {
            var spendings = await SpendingService.GetSpendingsForGroup(id);
            var publicFacing = SpendingService.ToSpendingData(spendings);
            return Ok(publicFacing);
        }

        /// <summary>
        /// Creates a new spending from the given parameters
        /// You should calculate the individual debts on the client side and send the result, the server only validates it
        /// </summary>
        /// <param name="newSpending">The new Spending to be added</param>
        /// <response code="403">Resource forbidden, current user is not a member of this group: 'user_not_member'</response>
        /// <response code="409">Business exception,
        /// adding same debtor multiple times: 'duplicate_debtor_id_found',
        /// invalid debtor id: 'not_all_debtors_are_members'</response>
        /// <response code="410">Resource gone,
        /// can't find current user in database: 'current_user_gone',
        /// can't find given group in database: 'group_gone',
        /// can't find given debtor in database: 'debtor_gone'</response>
        /// <response code="500">Internal error: 'spending_not_inserted'</response>
        [HttpPost("create")]
        public async Task<ActionResult<DaoSpending>> Create([FromBody] API.Request.NewSpending newSpending) {
            var created = await SpendingService.CreateNewSpending(newSpending, GetCurrentUserID());
            return Ok(created);
        }

#if DEBUG
        [Route("test1")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IList<DaoOptimizedDebt>>> Get() {
            return Ok(await SpendingService.GetOptimizedDebtForGroup(1));
        }
#endif

    }
}
