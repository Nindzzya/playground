using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iotX_Backend_Test.Classes
{
    public class FunctionsBase
    {
        public bool SWITCH { get; set; }
    }

    public class FlowFunctions : FunctionsBase
    {
        public int FLOW_VALUE { get; set; }
        public string HYDROMETER_STATUS { get; set; }
        public int HYDROMETER_VALUE { get; set; }
    }

   public class WindFunctions : FunctionsBase
    {
        public int FAN_SPEED { get; set; }
    }

    public class LightFunctions : FunctionsBase
    {
        public int LIGHT_INTENSITY { get; set; }
        public int EXT_LUX_LEVEL { get; set; }
    }
}
