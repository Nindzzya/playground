using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static UMS_Alpha.Core.Academic.ClassModule;
using static UMS_Alpha.Core.Academic.StudentModule;

namespace UMS_Alpha.Core.Academic
{
    public class AttendanceModule
    {
        public class ClassAttendance
        {
            public Class Class { get; set; }
            public int StudentsCount
            {
                get
                {
                    return Class.RegisteredCount;
                }
            }
            public List<bool> AttendanceCheck { get; set; }
            public DateTime Date { get; set; }
            public int Hour;
            public int SingleClassLength;
        }
    }
}
