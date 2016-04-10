using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntFactory.Sdk
{
    public class ProcessStageListResult
    {
        public List<ProcessStageEntity> processStages
        {
            set;
            get;
        }


        public int error_code = 0;

        public string error_message
        {
            get;
            set;
        }

    }
}
