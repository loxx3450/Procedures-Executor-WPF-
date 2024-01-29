using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using hw_23._02.Parameters;
using Microsoft.Data.SqlClient;

namespace hw_23._02.Handlers
{
    public class ProcedureHandler
    {
        private SqlCommand Command { get; set; }

        private ProceduresPreparator preparator = new();

        public List<SqlParameter> OutputParameters { get; set; } = new();

        public SqlDataReader Reader { get; set; }

        public bool ReaderIsOpened { get; set; }

        public async Task CreateProcedure(SqlConnection conn, string procedureName, ObservableCollection<Parameter> parameters, List<TextBox> values)
        {
            Command = preparator.GetCommand(conn, procedureName, parameters, values);

            CreateOutputParametersCollection(Command);

            Reader = await Command.ExecuteReaderAsync();

            ReaderIsOpened = true;
        }

        private void CreateOutputParametersCollection(SqlCommand cmd)
        {
            OutputParameters.Clear();

            foreach (SqlParameter parameter in cmd.Parameters)
            {
                if (parameter.Direction == ParameterDirection.Output)
                {
                    OutputParameters.Add(parameter);
                }
            }
        }

        ~ProcedureHandler()
        {
            if (ReaderIsOpened)
                Reader.DisposeAsync();
        }
    }
}
