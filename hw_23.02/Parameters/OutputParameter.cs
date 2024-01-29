using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw_23._02.Parameters
{
    public class OutputParameter
    {
        public string Name { get; set; }

        public SqlDbType DataType { get; set; }

        public object Value { get; set; }
    }
}
