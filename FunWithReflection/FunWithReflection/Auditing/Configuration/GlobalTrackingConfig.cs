using ClinicHQ.Data.Auditing.Exstensions;
using System;
using System.Linq.Expressions;

namespace ClinicHQ.Data.Auditing.Configuration
{
    public static class GlobalTrackingConfig
    {
        public static bool Enabled { get; set; } = true;

        public static bool TrackEmptyPropertiesOnAdditionAndDeletion { get; set; } = false;

        public static bool DisconnectedContext { get; set; } = false;

        internal static Type SoftDeletableType;
        internal static string SoftDeletablePropertyName;

        public static void SetSoftDeletableCriteria<TSoftDeletable>(
            Expression<Func<TSoftDeletable, bool>> softDeletableProperty)
        {
            SoftDeletableType = typeof(TSoftDeletable);
            SoftDeletablePropertyName = softDeletableProperty.GetPropertyInfo().Name;
        }
    }
}
