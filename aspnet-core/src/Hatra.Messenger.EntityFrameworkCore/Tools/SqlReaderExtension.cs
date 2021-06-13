using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FastMember;

namespace Hatra.Messenger.Tools
{
    public static class SqlReaderExtensions
    {
        public static async Task<List<T>> ToList<T>(this DbDataReader rd) where T : class, new()
        {
            var res = new List<T>();
            while (await rd.ReadAsync())
            {
                res.Add(rd.ConvertToObject<T>());
            }

            return res;
        }

        public static T ConvertToObject<T>(this DbDataReader rd) where T : class, new()
        {
            var type = typeof(T);
            var accessor = TypeAccessor.Create(type);
            var members = accessor.GetMembers();
            var t = new T();

            for (var i = 0; i < rd.FieldCount; i++)
            {
                if (!rd.IsDBNull(i))
                {
                    var fieldName = rd.GetName(i);

                    if (members.Any(m => string.Equals(m.Name, fieldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        accessor[t, fieldName] = rd.GetValue(i);
                    }
                }
            }

            return t;
        }
    }
}
