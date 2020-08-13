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
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private void btnPutIn_Click(object sender, RoutedEventArgs e)
        {
            model.StockIn("01.04.20");
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
    }
}
