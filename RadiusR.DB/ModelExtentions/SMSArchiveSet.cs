using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.ModelExtentions
{
    public static class SMSArchiveSet
    {
        public static SMSArchive AddSafely(this DbSet<SMSArchive> dbSet, SMSArchive entity)
        {
            if (entity == null)
                return null;

            return dbSet.Add(entity);
        }

        public static IEnumerable<SMSArchive> AddRangeSafely(this DbSet<SMSArchive> dbSet, IEnumerable<SMSArchive> entities)
        {
            var validEntities = entities.Where(sms => sms != null);
            if (validEntities.Any())
                return dbSet.AddRange(validEntities);
            return validEntities;
        }
    }
}
