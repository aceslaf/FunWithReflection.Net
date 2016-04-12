using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace FunWithReflection.Core
{
    public class AnonymusType<TEntity>
    {
        private Dictionary<string, object> _propertyValues;
        public AnonymusType()
        {
            _propertyValues = new Dictionary<string, object>();
        }
        public List<string> PropertyNames
        {
            get { return _propertyValues.Keys.ToList(); }
        }

        public T Get<T>(Expression<Func<TEntity, T>> propertyAccessor)
        {
            var name = NameOfProperty(propertyAccessor);
            if (_propertyValues.ContainsKey(name))
            {
                return (T)Convert.ChangeType(_propertyValues[name], typeof(T));
            }

            return default(T);
        }

        public AnonymusType<TEntity> Set<T>(Expression<Func<TEntity, T>> propertyAccessor, object value)
        {
            var name = NameOfProperty(propertyAccessor);
            _propertyValues.Add(name, value);
            return this;
        }

        public AnonymusType<TEntity> Set<T>(Expression<Func<TEntity, T>> propertyAccessor)
        {
            var name = NameOfProperty(propertyAccessor);
            _propertyValues.Add(name, default(T));
            return this;
        }

        private static string NameOfProperty<TEntity, T>(Expression<Func<TEntity, T>> propertyAccessor)
        {
            return ((MemberExpression)propertyAccessor.Body).Member.Name;
        }
    }
}
