using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation.MsSql
{
    internal static class SqlExtension
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ToSqlValue(this string text)
        {
            if (text == null)
            {
                return DBNull.Value;
            }
            else
            {
                return text;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static object ToSqlValue<T>(this Nullable<T> value)
            where T : struct
        {
            return value.HasValue ? value.Value as object : DBNull.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetString(this SqlDataReader reader, string name)
        {
            object value = reader[name];
            if (value == DBNull.Value)
            {
                return null;
            }
            else
            {
                return value.ToString();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTime? GetNullableDateTime(this SqlDataReader reader, string name)
        {
            object value = reader[name];
            if (value == DBNull.Value)
            {
                return null;
            }
            else
            {
                DateTime dtValue = (DateTime)value;
                return dtValue;
            }
        }
    }
}
