using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunWithReflection.Core
{
    public class CompositKey
    {
        private Dictionary<string, object> _keys;
        private readonly int _hash;
        public static CompositKey New(object key)
        {
            var keys = new List<KeyValuePair<string, object>>();
            foreach (var property in key.GetType().GetProperties())
            {
                keys.Add(new KeyValuePair<string, object>(property.Name, property.GetValue(key, null)));
            }
            return new CompositKey(keys);
        }
        private CompositKey(IEnumerable<KeyValuePair<string, object>> keys)
        {
            if (keys == null)
            {
                throw new ArgumentException("There has to be at leas one key");
            }
            if (!keys.Any())
            {
                throw new ArgumentException("There has to be at least one key");
            }

            int keyCount = keys.Count();
            long sum = 0;
            _keys = new Dictionary<string, object>();
            keys.ToList().ForEach(x =>
            {
                sum += x.GetHashCode() / keyCount;
                _keys.Add(x.Key, x.Value);
            });
            _hash = (int)sum;
        }

        public override int GetHashCode()
        {
            return _hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var other = obj as CompositKey;
            if (_keys.Count != other._keys.Count)
            {
                return false;
            }

            foreach (var key in _keys)
            {
                var ourKey = key.Value;
                var othersKey = other._keys[key.Key];
                if ((othersKey == null) != (ourKey == null))
                {
                    return false;
                }

                if (ourKey == null)
                {
                    continue;
                }

                if (!ourKey.Equals(othersKey))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
