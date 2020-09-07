using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
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
using Color = System.Windows.Media.Color;

namespace CS3D
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window, IWareHouseModel
    {
        WareHouseModel model = new WareHouseModel();
        bool conveyor_1, conveyor_2, conveyor_3, conveyor_4, conveyor_5, conveyor_6, conveyor_7, conveyor_8, conveyor_9, conveyor_10, conveyor_11,
            conveyor_12, conveyor_13, conveyor_14, conveyor_15, conveyor_16, conveyor_17, conveyor_18, conveyor_19, conveyor_20, conveyor_21, conveyor_22,
            ds_1, ds_2, ds_3, ds_4, ds_5, ds_6, ds_7, stacker1_1, stacker1_2, stacker1_3, stacker1_4, stacker2_1, stacker2_2, stacker2_3, stacker2_4;
        string missionId = "ProdIn";
        public Window1()
        {
            InitializeComponent();
            testControl.Content = model;
            model.GetProductmsg += Model_GetProductmsg;
            model.HintEvent += Model_HintEvent;
        }

        int count = 0;
        private void btnPutIn_Click(object sender, RoutedEventArgs e)
        {
            //model.ProdIn("ProdIn", "01.04.01");

            if (count == 0)
            {
                missionId = "ProdOut";
                model.ProdOut(missionId, "04.02.01");

                //missionId = "PalletOut";
                //model.PalletOut(missionId, "04.02.01");

            }
            else if (count == 1)
            {
                //missionId = "BackIn";
                //model.BackIn("BackIn", "04.02.01");
                //missionId = "GetPallet";
                //model.GetPallet(missionId, "04.02.01");
            }
            //else if (count == 2)
            //{
            //    missionId = "PalletIn";
            //    model.PalletIn(missionId, "04.02.01");
            //}
           

            //count++;
            //model.ProdTransfer("ProdTransfer","01.02.01", "01.05.05");
        }

        private void btnC_7_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_7 == false)
            {
                model.SetConveyor(missionId, "conveyor_7", true);
                btnC_7.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_7", false);
                btnC_7.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_7 = !conveyor_7;
        }

        private void btnC_6_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_6 == false)
            {
                model.SetConveyor(missionId, "conveyor_6", true);
                btnC_6.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_6", false);
                btnC_6.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_6 = !conveyor_6;
        }

        private void btnC_5_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_5 == false)
            {
                model.SetConveyor(missionId, "conveyor_5", true);
                btnC_5.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_5", false);
                btnC_5.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_5 = !conveyor_5;
        }

        private void btnC_22_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_22 == false)
            {
                model.SetConveyor(missionId, "conveyor_22", true);
                btnC_22.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_22", false);
                btnC_22.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_22 = !conveyor_22;
        }

        private void btnC_21_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_21 == false)
            {
                model.SetConveyor(missionId, "conveyor_21", true);
                btnC_21.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_21", false);
                btnC_21.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_21 = !conveyor_21;
        }

        private void btnC_16_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_16)
            {
                model.SetConveyor(missionId, "conveyor_16", true);
                btnC_16.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_16", false);
                btnC_16.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_16 = !conveyor_16;
        }

        private void btnC_13_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_13 == false)
            {
                model.SetConveyor(missionId, "conveyor_13", true);
                btnC_13.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_13", false);
                btnC_13.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_13 = !conveyor_13;
        }

        private void btnC_14_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_14 == false)
            {
                model.SetConveyor(missionId, "conveyor_14", true);
                btnC_14.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_14", false);
                btnC_14.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_14 = !conveyor_14;
        }

        private void btnC_19_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_19 == false)
            {
                model.SetConveyor(missionId, "conveyor_19", true);
                btnC_19.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_19", false);
                btnC_19.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_19 = !conveyor_19;
        }

        private void btnD_6_Click(object sender, RoutedEventArgs e)
        {
            if (ds_6 == false)
            {
                model.SetConveyor(missionId, "ds_6", true);
                btnD_6.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_6", false);
                btnD_6.Background = System.Windows.Media.Brushes.Red;
            }
            ds_6 = !ds_6;
        }

        private void btnS_24_Click(object sender, RoutedEventArgs e)
        {
            if (stacker2_4 == false)
            {
                model.SetConveyor(missionId, "stacker2_4", true);
                btnS_24.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker2_4", false);
                btnS_24.Background = System.Windows.Media.Brushes.Red;
            }
            stacker2_4 = !stacker2_4;
        }

        private void btnC_20_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_20 == false)
            {
                model.SetConveyor(missionId, "conveyor_20", true);
                btnC_20.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_20", false);
                btnC_20.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_20 = !conveyor_20;
        }

        private void btnD_7_Click(object sender, RoutedEventArgs e)
        {
            if (ds_7 == false)
            {
                model.SetConveyor(missionId, "ds_7", true);
                btnD_7.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_7", false);
                btnD_7.Background = System.Windows.Media.Brushes.Red;
            }
            ds_7 = !ds_7;
        }



        public void Model_GetProductmsg(string shelfNo, string shelfState, string productName, string productId, DateTime lastUpTime)
        {
            tbxShelfNo.Text = shelfNo;
            tbxShelfState.Text = shelfState;
            tbxProductName.Text = productName;
            tbxProductId.Text = productId;
            tbxLastUpTime.Text = lastUpTime.ToString();
        }


        public void Model_HintEvent(string msg)
        {
            MessageBox.Show(msg);
        }

        private void btnInitWareHouse_Click(object sender, RoutedEventArgs e)
        {
            model.Reset();//
        }

        private void btnC_1_Click(object sender, RoutedEventArgs e)
        {
            if (conveyor_1 == false)
            {
                model.SetConveyor(missionId, "conveyor_1", true);
                btnC_1.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_1", false);
                btnC_1.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_1 = !conveyor_1;


        }

        private void btnD_1_Click(object sender, RoutedEventArgs e)
        {
            if (!ds_1)
            {
                model.SetConveyor(missionId, "ds_1", true);
                btnD_1.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_1", false);
                btnD_1.Background = System.Windows.Media.Brushes.Red;
            }
            ds_1 = !ds_1;

        }

        private void btnC_3_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_3)
            {
                model.SetConveyor(missionId, "conveyor_3", true);
                btnC_3.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_3", false);
                btnC_3.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_3 = !conveyor_3;
        }

        private void btnC_4_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_4)
            {
                model.SetConveyor(missionId, "conveyor_4", true);
                btnC_4.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_4", false);
                btnC_4.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_4 = !conveyor_4;
        }

        private void btnS_1_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker1_1)
            {
                model.SetConveyor(missionId, "stacker1_1", true);
                btnS_1.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker1_1", false);
                btnS_1.Background = System.Windows.Media.Brushes.Red;

            }
            stacker1_1 = !stacker1_1;
        }

        private void btnS_2_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker1_2)
            {
                model.SetConveyor(missionId, "stacker1_2", true);
                btnS_2.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker1_2", false);
                btnS_2.Background = System.Windows.Media.Brushes.Red;

            }
            stacker1_2 = !stacker1_2;
        }

        private void btnS_3_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker1_3)
            {
                model.SetConveyor(missionId, "stacker1_3", true);
                btnS_3.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker1_3", false);
                btnS_3.Background = System.Windows.Media.Brushes.Red;

            }
            stacker1_3 = !stacker1_3;
        }

        private void btnC_8_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_8)
            {
                model.SetConveyor(missionId, "conveyor_8", true);
                btnC_8.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_8", false);
                btnC_8.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_8 = !conveyor_8;
        }

        private void btnD_3_Click(object sender, RoutedEventArgs e)
        {
            if (!ds_3)
            {
                model.SetConveyor(missionId, "ds_3", true);
                btnD_3.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_3", false);
                btnD_3.Background = System.Windows.Media.Brushes.Red;
            }
            ds_3 = !ds_3;
        }

        private void btnC_15_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_15)
            {
                model.SetConveyor(missionId, "conveyor_15", true);
                btnC_15.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_15", false);
                btnC_15.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_15 = !conveyor_15;
        }

        private void btnD_5_Click(object sender, RoutedEventArgs e)
        {
            if (!ds_5)
            {
                model.SetConveyor(missionId, "ds_5", true);
                btnD_5.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_5", false);
                btnD_5.Background = System.Windows.Media.Brushes.Red;
            }
            ds_5 = !ds_5;
        }

        private void btnC_18_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_18)
            {
                model.SetConveyor(missionId, "conveyor_18", true);
                btnC_18.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_18", false);
                btnC_18.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_18 = !conveyor_18;
        }

        private void btnS_21_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker2_1)
            {
                model.SetConveyor(missionId, "stacker2_1", true);
                btnS_21.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker2_1", false);
                btnS_21.Background = System.Windows.Media.Brushes.Red;
            }
            stacker2_1 = !stacker2_1;
        }

        private void btnS_22_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker2_2)
            {
                model.SetConveyor(missionId, "stacker2_2", true);
                btnS_22.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker2_2", false);
                btnS_22.Background = System.Windows.Media.Brushes.Red;
            }
            stacker2_2 = !stacker2_2;
        }

        private void btnS_23_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker2_3)
            {
                model.SetConveyor(missionId, "stacker2_3", true);
                btnS_23.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker2_3", false);
                btnS_23.Background = System.Windows.Media.Brushes.Red;
            }
            stacker2_3 = !stacker2_3;

        }

        private void btnC_17_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_17)
            {
                model.SetConveyor(missionId, "conveyor_17", true);
                btnC_17.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_17", false);
                btnC_17.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_17 = !conveyor_17;

        }

        private void btnS_4_Click(object sender, RoutedEventArgs e)
        {
            if (!stacker1_4)
            {
                model.SetConveyor(missionId, "stacker1_4", true);
                btnS_4.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "stacker1_4", false);
                btnS_4.Background = System.Windows.Media.Brushes.Red;
            }
            stacker1_4 = !stacker1_4;

        }

        private void btnC_10_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_10)
            {
                model.SetConveyor(missionId, "conveyor_10", true);
                btnC_10.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_10", false);
                btnC_10.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_10 = !conveyor_10;

        }

        private void btnC_11_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_11)
            {
                model.SetConveyor(missionId, "conveyor_11", true);
                btnC_11.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_11", false);
                btnC_11.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_11 = !conveyor_11;
        }

        private void btnC_12_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_12)
            {
                model.SetConveyor(missionId, "conveyor_12", true);
                btnC_12.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_12", false);
                btnC_12.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_12 = !conveyor_12;
        }

        private void btnD_4_Click(object sender, RoutedEventArgs e)
        {
            if (!ds_4)
            {
                model.SetConveyor(missionId, "ds_4", true);
                btnD_4.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_4", false);
                btnD_4.Background = System.Windows.Media.Brushes.Red;
            }
            ds_4 = !ds_4;
        }

        private void btnC_9_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_9)
            {
                model.SetConveyor(missionId, "conveyor_9", true);
                btnC_9.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_9", false);
                btnC_9.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_9 = !conveyor_9;
        }

        private void btnD_2_Click(object sender, RoutedEventArgs e)
        {
            if (!ds_2)
            {
                model.SetConveyor(missionId, "ds_2", true);
                btnD_2.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "ds_2", false);
                btnD_2.Background = System.Windows.Media.Brushes.Red;
            }
            ds_2 = !ds_2;
        }

        private void btnC_2_Click(object sender, RoutedEventArgs e)
        {
            if (!conveyor_2)
            {
                model.SetConveyor(missionId, "conveyor_2", true);
                btnC_2.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                model.SetConveyor(missionId, "conveyor_2", false);
                btnC_2.Background = System.Windows.Media.Brushes.Red;
            }
            conveyor_2 = !conveyor_2;
        }
    }
}
