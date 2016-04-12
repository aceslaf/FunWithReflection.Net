using ClinicHQ.Data.Auditing.Builders.Comparators;
using System.Data.Entity.Infrastructure;

namespace ClinicHQ.Data.Auditing.Builders
{
    public class ChangeLogDetailsBuilder : BaseLogDetailsBuilder
    {
        public ChangeLogDetailsBuilder(DbEntityEntry entry) : base(entry) { }

        public override void AfterSaveChanges()
        {

        }

        public override void BeforeSaveChanges()
        {
            CreateLogDetails();
        }

        protected override bool IsValueChanged(string propertyName)
        {
            var prop = DbEntry.Property(propertyName);
            var propertyType = DbEntry.Entity.GetType().GetProperty(propertyName).PropertyType;

            object originalValue = OriginalValue(propertyName);

            Comparator comparator = ComparatorFactory.GetComparator(propertyType);

            var changed = prop.IsModified && !comparator.AreEqual(CurrentValue(propertyName), originalValue);
            return changed;
        }
    }
}