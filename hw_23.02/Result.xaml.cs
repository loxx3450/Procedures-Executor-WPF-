using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using hw_23._02.Parameters;

namespace hw_23._02
{
    public partial class Result : Window
    {
        public DataView DataView { get; set; }
        public List<OutputParameter> OutputParameters { get; set; }

        public Result()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
