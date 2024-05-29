using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamLibarary
{
    internal class Student
    {
        [Index(1)]
        public int Id { get; set; }

        [Index(0)]
        public string Name { get; set; }

    }
}
