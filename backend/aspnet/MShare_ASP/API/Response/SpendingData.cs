﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response {

    /// <summary>
    /// Facing Data for a specific spending
    /// </summary>
    public class SpendingData {

        /// <summary>
        /// Name of the spending
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the spending
        /// </summary>
        public UserData Creditor { get; set; }

        /// <summary>
        /// Amount of money that has been spent
        /// </summary>
        public long MoneyOwed { get; set; }

        /// <summary>
        /// Debtors of this spending
        /// </summary>
        public IList<DebtorData> Debtors {get;set;}
    }
}
