using RadiusR.DB.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiusR.DB.BTKLogging
{
    public static class DBSetExtentions
    {
        public static IQueryable<Subscription> GetValidEntriesForClientCatalog(this DbSet<Subscription> dbSet, DateTime to)
        {
            var loggableStates = new[]
            {
                (short)CustomerState.Active,
                (short)CustomerState.Cancelled,
                (short)CustomerState.Disabled,
                (short)CustomerState.Reserved
            };
            return dbSet.Where(subscription => loggableStates.Contains(subscription.State) && subscription.MembershipDate <= to).OrderBy(s => s.ID).AsQueryable();
        }

        public static IQueryable<Subscription> GetValidEntriesForClientChanges(this DbSet<Subscription> dbSet, DateTime from, DateTime to)
        {
            return dbSet.Where(subscription => subscription.SystemLogs.Where(log => log.Date > from && log.Date <= to).Any(log => BTKLoggingUtilities.RelevantSystemLogTypes.Contains(log.LogType)) || subscription.SubscriptionStateHistories.Where(history=> history.ChangeDate > from && history.ChangeDate <= to).Any(history => history.NewState == (short)CustomerState.Cancelled || (history.NewState == (short)CustomerState.Disabled && history.OldState == (short)CustomerState.Active) || (history.OldState == (short)CustomerState.Disabled && history.NewState == (short)CustomerState.Active))).OrderBy(s => s.ID).AsQueryable();
        }
    }
}
