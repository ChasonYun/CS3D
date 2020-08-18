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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace CS3D
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        WareHouseModel model = new WareHouseModel();
        public Window1()
        {
            InitializeComponent();
            testControl.Content = model;
            model.GetProductmsg += Model_GetProductmsg;
        }

        private void Model_GetProductmsg(string shelfNo, string shelfState, string productName, string productId, DateTime lastUpTime)
        {
            tbxShelfNo.Text = shelfNo;
            tbxShelfState.Text = shelfState;
            tbxProductName.Text = productName;
            tbxProductId.Text = productId;
            tbxLastUpTime.Text = lastUpTime.ToString();
        }

        private void btnPutIn_Click(object sender, RoutedEventArgs e)
        {
            model.StockIn("06.04.01");
        }

        private void btnInitWareHouse_Click(object sender, RoutedEventArgs e)
        {
            model.Reset(MysqlDBHandler.Instance.GetStoreHouseState());//
        }

        private void btnC_1_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_1", true);
        }

        private void btnD_1_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("ds_1", true);
        }

     

        private void btnC_3_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_3", true);
        }

        private void btnC_4_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_4", true);
        }

        private void btnS_1_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker1_1", true);
        }

        private void btnS_2_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker1_2", true);
        }

        private void btnS_3_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker1_3", true);
        }

        private void btnC_8_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_8", true);
        }

        private void btnD_3_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("ds_3", true);
        }

        private void btnC_15_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_15", true);
        }

        private void btnD_5_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("ds_5", true);
        }

        private void btnC_18_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_18", true);
        }

        private void btnS_21_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker2_1", true);
        }

        private void btnS_22_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker2_2", true);
        }

        private void btnS_23_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("stacker2_3", true);
        }

        private void btnC_17_Click(object sender, RoutedEventArgs e)
        {
            model.SetConveyor("conveyor_17", true);
        }
    }
}
