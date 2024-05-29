using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class IndexAttribute : Attribute
    {
        public int Index { get; }

        public IndexAttribute(int index)
        {
            Index = index;
        }


        // 判斷是否存在index attribute，O(n)
        public static bool HasIndexAttribute<T>()
            where T : class, new()
        {
            bool result = true;
            var data = new T();
            PropertyInfo[] properties = data.GetType().GetProperties();

            foreach (var property in properties)
            {
                IndexAttribute indexAttribute = property.GetCustomAttribute<IndexAttribute>();
                result = result && (indexAttribute != null);
            }

            return result;
        }
    }
}
