using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hw_23._02.Handlers
{
    public class TypesConvertor
    {
        public SqlDbType ConvertTypeFromString(string type)
        {
            return type switch
            {
                "int" => SqlDbType.Int,
                "bit" => SqlDbType.Bit,
                _ => SqlDbType.NVarChar
            };
        }

        public object ConvertValueFromString(string value, SqlDbType type) 
        {
            return type switch
            {
                SqlDbType.Int => Convert.ToInt32(value),
                SqlDbType.Bit => Convert.ToBoolean(value),
                _ => value
            };
        }
    }
}
