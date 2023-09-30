using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxesProject.Conf
{
    public class ConfigData
    {
        public int EXPIRE_DAYS { get; set; }
        public int MAX_BOXES { get; set; }
        public int MIN_BOXES { get; set; }
        public double DEVIATION_PERCANTANGE { get; set; }
    }
}
