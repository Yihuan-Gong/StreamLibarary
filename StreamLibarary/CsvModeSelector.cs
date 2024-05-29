using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class CsvModeSelector
    {

        // 決定讀取/寫入模式
        // 1. 有header => 使用header
        // 2. 沒有header
        //    (1) 有index attribute => 使用index attribute
        //    (2) 沒index attribute => 按照property的順序
        public static CsvMode ModeSelector<T>(string path)
            where T : class, new()
        {
            CsvMode csvMode;

            if (CsvHeaderManager.HasHeader<T>(path))
            {
                csvMode = CsvMode.Header;
            }
            else
            {
                csvMode = IndexAttribute.HasIndexAttribute<T>() ? CsvMode.IndexAttribute : CsvMode.ByOrder;
            }

            return csvMode;
        }
    }
}
