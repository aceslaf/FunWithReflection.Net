﻿using ClinicHQ.Data.Auditing.Exstensions;
using System;

namespace ClinicHQ.Data.Auditing.Builders.Comparators
{
    internal static class ComparatorFactory
    {
        internal static Comparator GetComparator(Type type)
        {
            if (type.IsNullable<DateTime>())
            {
                return new NullableDateComparator();
            }

            if (type == typeof(DateTime))
            {
                return new DateComparator();
            }

            if (type == typeof(string))
            {
                return new StringComparator();
            }

            if (type.IsNullable())
            {
                return new NullableComparator();
            }

            if (type.IsValueType)
            {
                return new ValueTypeComparator();
            }

            return new Comparator();
        }
    }
}