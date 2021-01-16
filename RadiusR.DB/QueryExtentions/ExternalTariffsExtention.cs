using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB
{
    public static class ExternalTariffsExtention
    {
        public static IQueryable<ExternalTariff> GetActiveExternalTariffs(this DbSet<ExternalTariff> externalTariffs)
        {
            return externalTariffs.Include(et => et.Service).Where(et => et.Service.IsActive);
        }
    }
}
