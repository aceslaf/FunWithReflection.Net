using ClinicHQ.Data.Auditing.Builders.Comparators;
using ClinicHQ.Data.Auditing.Configuration;
using ClinicHQ.Data.Auditing.Exstensions;
using System.Data.Entity.Infrastructure;


namespace ClinicHQ.Data.Auditing.Builders
{
    public class DeletionLogDetailsBuilder : BaseLogDetailsBuilder
    {
        public DeletionLogDetailsBuilder(DbEntityEntry dbEntry) : base(dbEntry)
        {
        }

        protected override bool IsValueChanged(string propertyName)
        {
            if (GlobalTrackingConfig.TrackEmptyPropertiesOnAdditionAndDeletion)
                return true;

            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;
            object defaultValue = propertyType.DefaultValue();
            object orginalvalue = OriginalValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            return !comparator.AreEqual(defaultValue, orginalvalue);
        }

        protected override object CurrentValue(string propertyName)
        {
            return null;
        }

        public override void BeforeSaveChanges()
        {
            CreateLogDetails();
        }

        public override void AfterSaveChanges()
        {

        }
    }
}
