using FunWithReflection.External;
using System;
using System.Linq.Expressions;

namespace ClinicHQ.Data.Auditing.Configuration.Mapping
{
    class TypeSafeMap<T> : BaseMapp where T : EntityBase<int>
    {
        public TypeSafeMap(int typeId) : base(typeId) { }

        public override Type EntityType
        {
            get { return typeof(T); }
        }

        protected void Track<TProperty>(Expression<Func<T, TProperty>> propertySelector)
        {
            base.Track(propertySelector);
        }
    }
}
