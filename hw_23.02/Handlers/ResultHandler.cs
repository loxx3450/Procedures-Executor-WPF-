using hw_23._02.Parameters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace hw_23._02.Handlers
{
    internal class ResultHandler
    {
        public SqlDataReader Reader { get; set; }

        public List<SqlParameter> OutputParameters { get; set; }

        public void ShowResults()
        {
            DataTable dt = new DataTable();
            dt.Load(Reader);

            Result resultWindow = new Result()
            {
                DataView = dt.DefaultView,
                OutputParameters = GetOutputParameters()
            };

            resultWindow.ShowDialog();
        }

        private List<OutputParameter> GetOutputParameters()
        {
            List<OutputParameter> outputParameters = new();

            foreach (SqlParameter parameter in OutputParameters)
            {
                outputParameters.Add(new OutputParameter()
                {
                    Name = parameter.ParameterName,
                    DataType = parameter.SqlDbType,
                    Value = parameter.Value
                });
            }

            return outputParameters;
        }

        ~ResultHandler()
        {
            Reader.DisposeAsync();
        }
    }
}
