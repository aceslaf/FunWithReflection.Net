using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using ClinicHQ.Data.Auditing.Configuration;
using ClinicHQ.Data.Auditing.Exstensions;
using ClinicHQ.Data.Auditing.Builders.Comparators;

namespace ClinicHQ.Data.Auditing.Builders
{
    /// <summary>
    /// Creates AuditLogDetails for entries added in a previous call to SaveChanges.
    /// </summary>
    public class AdditionLogDetailsBuilder : BaseLogDetailsBuilder
    {
        public AdditionLogDetailsBuilder(DbEntityEntry dbEntry) : base(dbEntry)
        {
        }


        protected override IEnumerable<string> PropertyNamesOfEntity()
        {
            return DbEntry.CurrentValues.PropertyNames;
        }


        protected override bool IsValueChanged(string propertyName)
        {
            if (GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion)
                return true;

            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;
            object defaultValue = propertyType.DefaultValue();
            object currentValue = CurrentValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, currentValue);
        }

        protected override object OriginalValue(string propertyName)
        {
            return null;
        }

        public override void BeforeSaveChanges()
        {

        }

        public override void AfterSaveChanges()
        {
            CreateLogDetails();
        }
    }
}
