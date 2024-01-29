using hw_23._02.Handlers;
using hw_23._02.Parameters;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TextBox = System.Windows.Controls.TextBox;

namespace hw_23._02
{
    public partial class MainWindow : Window
    {
        //Connection
        private SqlConnection conn;
        private string connString;
        private const string dbName = "portal_db";

        //Convertor
        private TypesConvertor convertor = new();

        //Collections
        private List<TextBox> inputs = new List<TextBox>();
        public ObservableCollection<Parameter> Parameters { get; set; } = new();

        //Const Queries
        private const string getProceduresNamesQuery = "SELECT ROUTINE_NAME FROM portal_db.INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE = 'PROCEDURE' AND LEFT(ROUTINE_NAME, 3) NOT IN('sp_', 'xp_', 'ms_');";

        public MainWindow()
        {
            InitializeComponent();

            ConnectToDB();

            DataContext = this;
        }

        async private void ConnectToDB()
        {
            connString = ConfigurationManager.ConnectionStrings[dbName].ConnectionString;

            conn = new SqlConnection(connString);

            try
            {
                await conn.OpenAsync();

                await FillProceduresListBox();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task FillProceduresListBox()
        {
            SqlCommand cmd = new SqlCommand(getProceduresNamesQuery, conn);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                proceduresComboBox.Items.Add(reader.GetFieldValue<string>("ROUTINE_NAME"));
            }

            await reader.DisposeAsync();
        }

        private async void proceduresComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //query to get properties of Parameters
            string query = $"SELECT  P.name AS [ParameterName]  ,TYPE_NAME(P.user_type_id) AS[ParameterDataType]  , P.is_output AS [IsOutPutParameter] FROM sys.objects AS SO INNER JOIN sys.parameters AS P ON SO.OBJECT_ID = P.OBJECT_ID WHERE SO.name = '{proceduresComboBox.SelectedValue}'";

            SqlCommand cmd = new SqlCommand(query, conn);

            try
            {
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                Parameters.Clear();

                GetListOfParameters(reader).Result.ForEach((prm) => Parameters.Add(prm));

                FillStackPanel(Parameters.Count, Parameters);

                await reader.DisposeAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<Parameter>> GetListOfParameters(SqlDataReader reader)
        {
            List<Parameter> parameters = new List<Parameter>();

            string name;
            SqlDbType dataType;
            bool isOutput;

            while (await reader.ReadAsync())
            {
                name = reader.GetFieldValue<string>("ParameterName");
                dataType = convertor.ConvertTypeFromString(reader.GetFieldValue<string>("ParameterDataType"));          //Reader returns type as a string and convertor finds that type in SqlDbType enum
                isOutput = reader.GetFieldValue<bool>("IsOutPutParameter");

                parameters.Add(new Parameter() { Name = name, DataType = dataType, IsOutput = isOutput });
            }

            return parameters;
        }
    
        private void FillStackPanel(int count, ObservableCollection<Parameter> parameters)
        {
            parametersInputStackPanel.Children.Clear();
            inputs.Clear();

            for (int i = 0; i < count; i++) 
            {
                if (parameters[i].IsOutput)
                    continue;

                StackPanel inputField = new() { Margin = new Thickness(7) };

                inputField.Children.Add(new TextBlock() { Text = parameters[i].Name });

                TextBox textBox = new TextBox();

                inputField.Children.Add(textBox);
                inputs.Add(textBox);

                parametersInputStackPanel.Children.Add(inputField);
            }
        }

        private async void ExecProcedureBtn_Click(object sender, RoutedEventArgs e)
        {
            ProcedureHandler procHandler = new ProcedureHandler();
            
            try
            {
                await procHandler.CreateProcedure(conn, Convert.ToString(proceduresComboBox.SelectedValue), Parameters, inputs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unhandled exception just occurred: " + ex.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (procHandler.ReaderIsOpened)
            {
                ResultHandler resultHandler = new ResultHandler()
                {
                    Reader = procHandler.Reader,
                    OutputParameters = procHandler.OutputParameters
                };

                resultHandler.ShowResults();
            }
        }

        ~MainWindow()
        {
            conn.CloseAsync();
        }
    }

    
}
