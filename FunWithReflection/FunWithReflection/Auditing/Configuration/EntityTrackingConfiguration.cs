using ClinicHQ.Data.Auditing.Configuration.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ClinicHQ.Data.Auditing.Configuration
{
    public static class EntityTrackingConfiguration
    {
        private static Dictionary<Type, BaseMapp> mappings;
        private static Dictionary<int, Type> reverseMap;

        static EntityTrackingConfiguration()
        {
            mappings = new Dictionary<Type, BaseMapp>();
            reverseMap = new Dictionary<int, Type>();
        }

        public static void Map(BaseMapp mapping)
        {
            if (mappings.ContainsKey(mapping.EntityType))
            {
                throw new ArgumentException(string.Format("There already exists a mapping for the entity type:{0}", mapping.EntityType));
            }

            mappings.Add(mapping.EntityType, mapping);
            reverseMap.Add(mapping.TypeId, mapping.EntityType);
        }



        internal static bool IsPropertyTracked(Type entityType, string propertyName)
        {
            return
            mappings.ContainsKey(entityType)
            &&
            mappings[entityType].IsPropertyTracked(propertyName);
        }

        public static bool IsTrackingEnabled(Type entityType)
        {
            return mappings.ContainsKey(entityType);
        }

        public static IEnumerable<Type> TrackedTypes
        {
            get { return mappings.Keys.ToList(); }
        }

        public static Type GetTypeById(int typeId)
        {
            return reverseMap[typeId];
        }

        public static int GetTypeId<TEntity>()
        {
            return GetTypeId(typeof(TEntity));
        }

        public static int GetTypeId(Type entityType)
        {
            return mappings[entityType].TypeId;
        }
    }
}