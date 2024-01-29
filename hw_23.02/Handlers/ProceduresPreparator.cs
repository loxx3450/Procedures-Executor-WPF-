using hw_23._02.Parameters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace hw_23._02.Handlers
{
    internal class ProceduresPreparator
    {
        private TypesConvertor convertor = new();

        public SqlCommand GetCommand(SqlConnection conn, string procedureName, ObservableCollection<Parameter> parameters, List<TextBox> values)
        {
            SqlCommand cmd = new SqlCommand(procedureName, conn)
            {
                CommandType = CommandType.StoredProcedure
            };

            GetSqlParameters(parameters, values).ForEach((prm) => cmd.Parameters.Add(prm));

            return cmd;
        }

        private List<SqlParameter> GetSqlParameters(ObservableCollection<Parameter> parameters, List<TextBox> values)
        {
            List<SqlParameter> sqlParameters = new();

            int valuesCount = 0;

            for (int i = 0; i < parameters.Count; ++i)
            {
                SqlParameter temp = new()
                {
                    ParameterName = parameters[i].Name,
                    SqlDbType = parameters[i].DataType,
                };

                if (parameters[i].IsOutput)
                    temp.Direction = ParameterDirection.Output;
                else
                    temp.Value = convertor.ConvertValueFromString(values[valuesCount++].Text, parameters[i].DataType);              //Converts that string to needable type

                sqlParameters.Add(temp);
            }

            return sqlParameters;
        }
    }
}
