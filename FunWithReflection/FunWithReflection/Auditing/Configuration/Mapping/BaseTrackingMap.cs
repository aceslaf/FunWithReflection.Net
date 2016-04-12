using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ClinicHQ.Data.Auditing.Configuration.Mapping
{
    public abstract class BaseMapp
    {
        public abstract Type EntityType { get; }
        public int TypeId { get; private set; }
        private ISet<string> TrackedProperties { get; set; }

        public BaseMapp(int typeId)
        {
            TrackedProperties = new HashSet<string>();
            TypeId = typeId;
        }

        public bool IsPropertyTracked(string propertyName)
        {
            return TrackedProperties.Contains(propertyName);
        }

        protected void Track<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> propertySelector)
        {
            if (propertySelector == null)
            {
                throw new ArgumentNullException(String.Format("{0} is null", nameof(propertySelector)));
            }

            var name = ((MemberExpression)propertySelector.Body).Member.Name;
            AddTrackedProperty(name);
        }

        private void AddTrackedProperty(string propertyName)
        {
            TrackedProperties.Add(propertyName);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var objAsTypeMap = obj as BaseMapp;
            if (objAsTypeMap == null)
            {
                return false;
            }


            return EntityType.Equals(objAsTypeMap.EntityType);
        }

        public override int GetHashCode()
        {
            return EntityType.GetHashCode();
        }
    }
}
