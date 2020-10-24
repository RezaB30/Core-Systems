﻿using RezaB.API.MobilExpress.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.API.MobilExpress.DBAdapter.AdapterClient
{
    public class MobilExpressAdapterClientGetCardsResponse : MobilExpressAdapterClientResponseBase
    {
        public new MobilExpressCardListResponse Response { get; set; }
    }
}
