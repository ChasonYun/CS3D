using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CS3D
{
    /// <summary>
    /// WareHouseModel.xaml 的交互逻辑
    /// </summary>
    public partial class WareHouseModel : UserControl
    {
        public delegate void HintEventHandler(string msg);

        public event HintEventHandler HintEvent;


        public delegate void GetProductMsg(string shelfNo, string shelfState, string productName, string productId, DateTime lastUpTime);

        public event GetProductMsg GetProductmsg;


        private Point3D initPosition;//记录相机初始信息
        private Vector3D initLookDirection;
        private Vector3D initUpDirection;
        private double initFieldofView;

        private ModelPosition modelPosition;//模型  位置信息

      

        private Timer timer = new Timer();

        /// <summary>
        /// 全局模型  
        /// </summary>
        private Dictionary<string, ProductInfo> product_Info = new Dictionary<string, ProductInfo>();
        private Dictionary<string, StackerPartsInfo> stackerParts_Info = new Dictionary<string, StackerPartsInfo>();
        private Dictionary<string, ShelfInfo> shelf_Info = new Dictionary<string, ShelfInfo>();

        IMission mission;
        /// <summary>
        /// 任务队列
        /// </summary>
        List<IMission> missionList = new List<IMission>();
        private readonly object obj_missionList = new object();

        public WareHouseModel()
        {
            try
            {
                InitializeComponent();
                InitSetData();
                InitColor();
                InitModel();
                timer.Elapsed += Timer_Elapsed;
                timer.Interval = 50;
                timer.Start();
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel has exception:" + ex.ToString()));
            }
        }
        private void InitSetData()
        {
            try
            {
                modelPosition = new ModelPosition();

                initPosition = perspectiveCamera.Position;
                initLookDirection = perspectiveCamera.LookDirection;
                initUpDirection = perspectiveCamera.UpDirection;
                initFieldofView = perspectiveCamera.FieldOfView;

                StackerPartsInfo zaihuotai001 = new StackerPartsInfo("zaihuotai001");
                stackerParts_Info.Add(zaihuotai001.ModelName, zaihuotai001);

                StackerPartsInfo duiduojilizhu001 = new StackerPartsInfo("duiduojilizhu001");
                stackerParts_Info.Add(duiduojilizhu001.ModelName, duiduojilizhu001);

                StackerPartsInfo shangcha001 = new StackerPartsInfo("shangcha001");
                stackerParts_Info.Add(shangcha001.ModelName, shangcha001);

                StackerPartsInfo xiacha001 = new StackerPartsInfo("xiacha001");
                stackerParts_Info.Add(xiacha001.ModelName, xiacha001);

                StackerPartsInfo zhongcha001 = new StackerPartsInfo("zhongcha001");
                stackerParts_Info.Add(zhongcha001.ModelName, zhongcha001);

                StackerPartsInfo VIFS001 = new StackerPartsInfo("VIFS001");
                stackerParts_Info.Add(VIFS001.ModelName, VIFS001);

                StackerPartsInfo zaihuotai002 = new StackerPartsInfo("zaihuotai002");
                stackerParts_Info.Add(zaihuotai002.ModelName, zaihuotai002);

                StackerPartsInfo duiduojilizhu002 = new StackerPartsInfo("duiduojilizhu002");
                stackerParts_Info.Add(duiduojilizhu002.ModelName, duiduojilizhu002);

                StackerPartsInfo shangcha002 = new StackerPartsInfo("shangcha002");
                stackerParts_Info.Add(shangcha002.ModelName, shangcha002);

                StackerPartsInfo xiacha002 = new StackerPartsInfo("xiacha002");
                stackerParts_Info.Add(xiacha002.ModelName, xiacha002);

                StackerPartsInfo zhongcha002 = new StackerPartsInfo("zhongcha002");
                stackerParts_Info.Add(zhongcha002.ModelName, zhongcha002);

                StackerPartsInfo VIFS002 = new StackerPartsInfo("VIFS002");
                stackerParts_Info.Add(VIFS002.ModelName, VIFS002);
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_InitGetXmlData has exception:" + ex.ToString()));
            }
        }


        ///刷子工具
        DiffuseMaterial diffMat_VIFS;
        DiffuseMaterial diffMat_zaihuotai;
        DiffuseMaterial diffMat_Prism;

        DiffuseMaterial diffMat_duiduojilizhu;
        DiffuseMaterial diffMat_BASEBOX;
        DiffuseMaterial diffMat_shangcha;
        DiffuseMaterial diffMat_zhongcha;
        DiffuseMaterial diffMat_xiacha;


        DiffuseMaterial diffMat_conveyor7;
        DiffuseMaterial diffMat_chain7;

        DiffuseMaterial diffMat_conveyor6;
        DiffuseMaterial diffMat_chain6;

        DiffuseMaterial diffMat_conveyor1;
        DiffuseMaterial diffMat_roller1;

        DiffuseMaterial diffMat_obj178;
        DiffuseMaterial diffMat_sick1;
        DiffuseMaterial diffMat_sick2;
        DiffuseMaterial diffMat_track_top;
        DiffuseMaterial diffMat_track_bottom;

        DiffuseMaterial diffMat_obj36_56;

        DiffuseMaterial diffMat_rack;
        DiffuseMaterial diffMat_BOX;
        DiffuseMaterial diffMat_goods;
        DiffuseMaterial diffMat_Pallet;
        DiffuseMaterial diffMat_Ground;
        DiffuseMaterial diffMat_track001;
        DiffuseMaterial diffMat_obj036;
        DiffuseMaterial diffMat_laser;
        DiffuseMaterial diffMat_obj018;
        /// <summary>
        /// 初始化刷子工具
        /// </summary>
        private void InitBrush()
        {

            try
            {
                //ImageBrush brushDuiduojilizhu = new ImageBrush();
                //brushDuiduojilizhu.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303099.jpg", UriKind.Relative));
                //brushDuiduojilizhu.TileMode = TileMode.Tile;
                diffMat_duiduojilizhu = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(224, 145, 92)));

                diffMat_BASEBOX = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303096.jpg", UriKind.Relative))));
                diffMat_shangcha = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303059.jpg", UriKind.Relative))));
                diffMat_zhongcha = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303059.jpg", UriKind.Relative))));
                diffMat_xiacha = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303059.jpg", UriKind.Relative))));




                diffMat_obj36_56 = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303097.jpg", UriKind.Relative))));

                diffMat_rack = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM303050.jpg", UriKind.Relative))));
                diffMat_BOX = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/115.jpg", UriKind.Relative))));

                //货物
                ImageBrush brushGoods = new ImageBrush();
                brushGoods.ImageSource = new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/face04-2.jpg", UriKind.Relative));
                brushGoods.TileMode = TileMode.Tile;
                diffMat_goods = new DiffuseMaterial(brushGoods);

                diffMat_VIFS = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(203, 210, 239)));
                diffMat_zaihuotai = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(200, 164, 134)));



                diffMat_Pallet = new DiffuseMaterial(new ImageBrush(new BitmapImage(new Uri(Environment.CurrentDirectory + "/image/CM91450h.jpg", UriKind.Relative))));
                diffMat_Ground = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(255, 255, 255)));
                diffMat_conveyor7 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(173, 175, 154)));
                diffMat_chain7 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(30, 30, 30)));
                diffMat_conveyor6 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(95, 80, 80)));
                diffMat_chain6 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(30, 30, 30)));
                diffMat_conveyor1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(128, 128, 128)));
                diffMat_roller1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(129, 47, 231)));
                diffMat_obj178 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(128, 128, 128)));
                diffMat_sick1 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(19, 152, 210)));
                diffMat_sick2 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(39, 39, 39)));
                diffMat_Prism = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(119, 228, 12)));
                diffMat_track_top = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(15, 15, 15)));
                diffMat_track_bottom = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(173, 173, 173)));
                diffMat_track001 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(141, 141, 141)));
                diffMat_obj036 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(173, 173, 173)));
                diffMat_laser = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(24, 24, 24)));
                diffMat_obj018 = new DiffuseMaterial(new SolidColorBrush(Color.FromRgb(173, 173, 173)));
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_InitBrush has exception:" + ex.ToString()));
            }
            //堆垛机立柱

        }

        /// <summary>
        /// 初始化渲染 表面
        /// </summary>
        private void InitColor()
        {
            InitBrush();
            try
            {
                duiduojilizhu_001.Material = diffMat_duiduojilizhu;
                duiduojilizhu_002.Material = diffMat_duiduojilizhu;
                zaihuotai_001.Material = diffMat_duiduojilizhu;
                zaihuotai_002.Material = diffMat_duiduojilizhu;

                shangcha_001.Material = diffMat_shangcha;
                shangcha_002.Material = diffMat_shangcha;
                xiacha_001.Material = diffMat_xiacha;
                xiacha_002.Material = diffMat_xiacha;
                zhongcha_001.Material = diffMat_zhongcha;
                zhongcha_002.Material = diffMat_zhongcha;
                VIFS_001.Material = diffMat_VIFS;
                VIFS_002.Material = diffMat_VIFS;




                BASEBOX_001.Material = diffMat_BASEBOX;
                BASEBOX_002.Material = diffMat_BASEBOX;

                conveyor_7.Material = diffMat_conveyor7;
                chain_7.Material = diffMat_chain7;
                conveyor_14.Material = diffMat_conveyor7;
                chain_14.Material = diffMat_chain7;

                conveyor_6.Material = diffMat_conveyor6;
                chain_6.Material = diffMat_chain6;
                conveyor_5.Material = diffMat_conveyor6;
                chain_5.Material = diffMat_chain6;
                conveyor_4.Material = diffMat_conveyor6;
                chain_4.Material = diffMat_chain6;
                conveyor_3.Material = diffMat_conveyor6;
                chain_3.Material = diffMat_chain6;
                conveyor_13.Material = diffMat_conveyor6;
                chain_13.Material = diffMat_chain6;
                conveyor_12.Material = diffMat_conveyor6;
                chain_12.Material = diffMat_chain6;
                conveyor_11.Material = diffMat_conveyor6;
                chain_11.Material = diffMat_chain6;
                conveyor_10.Material = diffMat_conveyor6;
                chain_10.Material = diffMat_chain6;
                conveyor_19.Material = diffMat_conveyor6;
                chain_19.Material = diffMat_chain6;
                conveyor_18.Material = diffMat_conveyor6;
                chain_18.Material = diffMat_chain6;
                conveyor_17.Material = diffMat_conveyor6;
                chain_17.Material = diffMat_chain6;
                conveyor_22.Material = diffMat_conveyor6;
                chain_22.Material = diffMat_chain6;
                conveyor_23.Material = diffMat_conveyor6;
                chain_23.Material = diffMat_chain6;
                conveyor_21.Material = diffMat_conveyor6;
                chain_21.Material = diffMat_chain6;

                conveyor_1.Material = diffMat_conveyor1;
                roller_1.Material = diffMat_roller1;
                conveyor_2.Material = diffMat_conveyor1;
                roller_2.Material = diffMat_roller1;
                conveyor_ds2.Material = diffMat_conveyor1;
                roller_ds2.Material = diffMat_roller1;
                conveyor_ds1.Material = diffMat_conveyor1;
                roller_ds1.Material = diffMat_roller1;
                conveyor_9.Material = diffMat_conveyor1;
                roller_9.Material = diffMat_roller1;
                conveyor_8.Material = diffMat_conveyor1;
                roller_8.Material = diffMat_roller1;
                conveyor_ds4.Material = diffMat_conveyor1;
                roller_ds4.Material = diffMat_roller1;
                conveyor_ds3.Material = diffMat_conveyor1;
                roller_ds3.Material = diffMat_roller1;
                conveyor_16.Material = diffMat_conveyor1;
                roller_16.Material = diffMat_roller1;
                conveyor_15.Material = diffMat_conveyor1;
                roller_15.Material = diffMat_roller1;
                conveyor_ds6.Material = diffMat_conveyor1;
                roller_ds6.Material = diffMat_roller1;
                conveyor_ds5.Material = diffMat_conveyor1;
                roller_ds5.Material = diffMat_roller1;
                conveyor_20.Material = diffMat_conveyor1;
                roller_20.Material = diffMat_roller1;
                conveyor_ds7.Material = diffMat_conveyor1;
                roller_ds7.Material = diffMat_roller1;
                track_top_1.Material = diffMat_track_top;
                track_top_2.Material = diffMat_track_top;
                track_bottom_1.Material = diffMat_track_bottom;
                track_bottom_2.Material = diffMat_track_bottom;
                rack_001.Material = diffMat_track001;
                rack_002.Material = diffMat_track001;
                obj_058.Material = diffMat_track001;
                obj_036.Material = diffMat_obj036;
                obj_056.Material = diffMat_obj036;
                obj_037.Material = diffMat_obj036;
                obj_057.Material = diffMat_obj036;
                laser_left_1.Material = diffMat_laser;
                laser_left_2.Material = diffMat_laser;
                laser_right_1.Material = diffMat_laser;
                laser_right_2.Material = diffMat_laser;
                //Led_zhijia_042.Material = diffMat_conveyor;入
                //Led_zhijia_043.Material = diffMat_conveyor;
                obj_178.Material = diffMat_obj178;
                obj_179.Material = diffMat_obj178;
                obj_018.Material = diffMat_obj018;
                obj_038.Material = diffMat_obj018;
                obj_059.Material = diffMat_obj018;
                sick_11.Material = diffMat_sick1;
                sick_21.Material = diffMat_sick1;
                sick_12.Material = diffMat_sick2;
                sick_22.Material = diffMat_sick2;


                Prism_005.Material = diffMat_Prism;
                Prism_006.Material = diffMat_Prism;
                Prism_007.Material = diffMat_Prism;
                Prism_008.Material = diffMat_Prism;


            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_InitColor has exception:" + ex.ToString()));
            }

        }

        private void InitModel()
        {
            try
            {
                for (int i = 1; i <= 17; i++)
                {
                    ModelVisual3D cloneModelShelf = new ModelVisual3D();
                    cloneModelShelf.Content = obj018.Content.Clone();
                    RootGeometryContainer.Children.Add(cloneModelShelf);
                    ShelfInfo shelfInfo = new ShelfInfo("obj0" + (18 + i));
                    shelfInfo.Model = cloneModelShelf;
                    shelfInfo.ShelfOffSet = new Point3D(modelPosition.ShelfDistance * i, 0, 0);
                    shelf_Info.Add(shelfInfo.ModelName, shelfInfo);

                    ModelVisual3D cloneModelShelf_ = new ModelVisual3D();
                    cloneModelShelf_.Content = obj038.Content.Clone();
                    RootGeometryContainer.Children.Add(cloneModelShelf_);
                    ShelfInfo shelfInfo_ = new ShelfInfo("obj0" + (38 + i));
                    shelfInfo_.Model = cloneModelShelf_;
                    shelfInfo_.ShelfOffSet = new Point3D(modelPosition.ShelfDistance * i, 0, 0);
                    shelf_Info.Add(shelfInfo_.ModelName, shelfInfo_);

                    ModelVisual3D _cloneModelShelf = new ModelVisual3D();
                    _cloneModelShelf.Content = obj059.Content.Clone();
                    RootGeometryContainer.Children.Add(_cloneModelShelf);
                    ShelfInfo _shelfInfo = new ShelfInfo("obj0" + (59 + i));
                    _shelfInfo.Model = _cloneModelShelf;
                    _shelfInfo.ShelfOffSet = new Point3D(modelPosition.ShelfDistance * i, 0, 0);
                    shelf_Info.Add(_shelfInfo.ModelName, _shelfInfo);
                }
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_InitModel has exception:" + ex.ToString()));
            }

        }

        /// <summary>
        /// 鼠标滚轮事件 缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        Point tempPoint = new Point();
        Point3D cameraPosition = new Point3D();
        Vector3D cameraLookDirection = new Vector3D();

        /// <summary>
        /// 鼠标移动 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VP3D_MouseMove(object sender, MouseEventArgs e)
        {
            //if (e.LeftButton == MouseButtonState.Pressed)
            //{
            //    Point point = Mouse.GetPosition(e.Source as FrameworkElement);
            //    cameraLookDirection = perspectiveCamera.LookDirection;
            //    perspectiveCamera.LookDirection = new Vector3D((-point.X + tempPoint.X) + cameraLookDirection.X, (-point.Y + tempPoint.Y) + cameraLookDirection.Y, cameraLookDirection.Z);
            //    cameraLookDirection = perspectiveCamera.LookDirection;
            //}
        }

        private void VP3D_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                tempPoint = Mouse.GetPosition(e.Source as FrameworkElement);
                VP3D.Focus();
                tempPoint = e.GetPosition(VP3D);
                PointHitTestParameters hitTestParameters = new PointHitTestParameters(tempPoint);
                VisualTreeHelper.HitTest(VP3D, null, ResultCallback, hitTestParameters);
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_ResultCallback has exception:" + ex.ToString()));
            }

        }

        public HitTestResultBehavior ResultCallback(HitTestResult result)
        {
            try
            {
                var tempModel = result.VisualHit as ModelVisual3D;
                if (tempModel != null)
                {

                }
                RayHitTestResult rayHitTest = result as RayHitTestResult;
                if (rayHitTest != null)
                {
                    RayMeshGeometry3DHitTestResult rayMeshGeometry3DHitTestResult = rayHitTest as RayMeshGeometry3DHitTestResult;
                    if (rayMeshGeometry3DHitTestResult != null)
                    {
                        foreach (var temp in product_Info.Values)
                        {
                            if (temp.Model.Content == rayMeshGeometry3DHitTestResult.ModelHit)
                            {
                                GetProductmsg(temp.ShelfNo, temp.ShelfState.ToString(), temp.ProductName, temp.ProductId, temp.LastUpTime);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw new Exception("ResultCallback" + ex.ToString());
            }

            return HitTestResultBehavior.Continue;
        }

        private void VP3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    perspectiveCamera.FieldOfView -= 10;
                    if (perspectiveCamera.FieldOfView <= 0)
                    {
                        perspectiveCamera.FieldOfView = 1;
                    }
                }
                else if (e.Delta < 0)
                {
                    perspectiveCamera.FieldOfView += 10;
                    if (perspectiveCamera.FieldOfView > 180)
                    {
                        perspectiveCamera.FieldOfView = 179;
                    }
                }
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_VP3D_MouseWheel has exception:" + ex.ToString()));

            }

        }
        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            perspectiveCamera.Position = initPosition;
            perspectiveCamera.LookDirection = initLookDirection;
            perspectiveCamera.UpDirection = initUpDirection;
            perspectiveCamera.FieldOfView = initFieldofView;

        }

        int testi = 0;

        /// <summary>
        /// 测试按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest_Click(object sender, RoutedEventArgs e)
        {


            testi++;
            if (testi == 1)
            {
                perspectiveCamera.Position = new Point3D(perspectiveCamera.Position.X + 1000, perspectiveCamera.Position.Y, perspectiveCamera.Position.Z);
            }
            if (testi == 2)
            {
                perspectiveCamera.Position = new Point3D(perspectiveCamera.Position.X, perspectiveCamera.Position.Y + 1000, perspectiveCamera.Position.Z);
            }
            if (testi == 3)
            {
                perspectiveCamera.Position = new Point3D(perspectiveCamera.Position.X, perspectiveCamera.Position.Y, perspectiveCamera.Position.Z + 1000);
            }
            if (testi == 4)
            {
                xiacha002.Content.Transform = new TranslateTransform3D() { OffsetX = 1000 };
            }
            if (testi == 5)
            {
                zhongcha002.Content.Transform = new TranslateTransform3D() { OffsetX = 2000 };
            }
            if (testi == 6)
            {
                shangcha002.Content.Transform = new TranslateTransform3D() { OffsetX = 3000 };
            }

            //ModelVisual3D obj = (ModelVisual3D)this.FindName("test");
            //_01_01_02_.Transform.Clone().Transform(new Point3D(0,-500,0));
        }

        /// <summary>
        /// 快捷键  操作 相机  position  lookdirection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VP3D_KeyDown(object sender, KeyEventArgs e)
        {
            cameraPosition = new Point3D(perspectiveCamera.Position.X, perspectiveCamera.Position.Y, perspectiveCamera.Position.Z);
            cameraLookDirection = perspectiveCamera.LookDirection;
            if (Keyboard.IsKeyDown(Key.E) && Keyboard.IsKeyDown(Key.W))
            {
                perspectiveCamera.LookDirection = new Vector3D(cameraLookDirection.X - 400, cameraLookDirection.Y, cameraLookDirection.Z);
            }
            else if (Keyboard.IsKeyDown(Key.Q) && Keyboard.IsKeyDown(Key.W))
            {
                perspectiveCamera.LookDirection = new Vector3D(cameraLookDirection.X + 400, cameraLookDirection.Y, cameraLookDirection.Z);
            }

            else if (Keyboard.IsKeyDown(Key.A) && Keyboard.IsKeyDown(Key.W))
            {
                perspectiveCamera.LookDirection = new Vector3D(cameraLookDirection.X, cameraLookDirection.Y - 400, cameraLookDirection.Z);
            }
            else if (Keyboard.IsKeyDown(Key.D) && Keyboard.IsKeyDown(Key.W))
            {
                perspectiveCamera.LookDirection = new Vector3D(cameraLookDirection.X + 200, cameraLookDirection.Y + 400, cameraLookDirection.Z);
            }

            else if (Keyboard.IsKeyDown(Key.D))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X - 200, cameraPosition.Y, cameraPosition.Z);
            }
            else if (Keyboard.IsKeyDown(Key.A))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X + 200, cameraPosition.Y, cameraPosition.Z);
            }
            else if (Keyboard.IsKeyDown(Key.W))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X, cameraPosition.Y - 200, cameraPosition.Z);
            }

            else if (Keyboard.IsKeyDown(Key.S))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X, cameraPosition.Y + 200, cameraPosition.Z);
            }
            else if (Keyboard.IsKeyDown(Key.Q))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z + 200);
            }
            else if (Keyboard.IsKeyDown(Key.E))
            {
                perspectiveCamera.Position = new Point3D(cameraPosition.X, cameraPosition.Y, cameraPosition.Z - 200);
            }
            cameraLookDirection = perspectiveCamera.LookDirection;
            cameraPosition = new Point3D(perspectiveCamera.Position.X, perspectiveCamera.Position.Y, perspectiveCamera.Position.Z);
        }



        /// <summary>
        /// 刷新仓库状态 维护 模型数据字典
        /// </summary>
        public void Reset()
        {
            try
            {
                DataTable StoreHouseStateDT = MysqlDBHandler.Instance.GetStoreHouseState();
                if (StoreHouseStateDT != null)
                {
                    ProductInfo info;
                    //int tempState;
                    DataRow[] dataRows = StoreHouseStateDT.Select("ShelfState='1'");
                    foreach (DataRow dataRow in dataRows)
                    {
                        info = new ProductInfo();
                        info.ShelfNo = dataRow[1].ToString();
                        info.ShelfState = Convert.ToInt32(dataRow[2].ToString());
                        info.ProductName = dataRow[3].ToString();
                        info.ProductId = Convert.ToString(dataRow[4]);
                        info.LastUpTime = Convert.ToDateTime(dataRow[5].ToString());
                        info.ProductOffSet = modelPosition.Get_ShelfState_1_OffSet(info.ShelfNo);

                        ModelVisual3D cloneModel = new ModelVisual3D();
                        cloneModel.Content = originProduct.Content.Clone();
                        RootGeometryContainer.Children.Add(cloneModel);

                        info.Model = cloneModel;
                        if (!product_Info.ContainsKey(info.ShelfNo))
                        {
                            product_Info.Add(info.ShelfNo, info);
                        }
                    }
                }
                //temp = RootGeometryContainer.Children.Count();
            }
            catch (Exception ex)
            {
                HintEvent(string.Format("WareHouseModel_Reset has exception:" + ex.ToString()));
            }

        }

        /// <summary>
        /// 入库
        /// </summary>
        /// <param name="shelfNo"></param>
        public void StockIn(string shelfNo)
        {
            if (!product_Info.ContainsKey(shelfNo))
            {
                ProductInfo productInfo = new ProductInfo();

                ModelVisual3D cloneModel = new ModelVisual3D();
                cloneModel.Content = originProduct.Content.Clone();
                RootGeometryContainer.Children.Add(cloneModel);

                productInfo.ShelfNo = shelfNo;
                productInfo.Model = cloneModel;
                productInfo.ProductOffSet = (Point3D)(modelPosition.StockInEntranceOriPos - modelPosition.ProductOriPos);//入货口初始位置
                product_Info.Add(shelfNo, productInfo);
                mission = new Mission(MissionType.StockIn, shelfNo, modelPosition, product_Info, stackerParts_Info);
                mission.MisSuccess += Mission_MisSuccess;
                lock (obj_missionList)
                {
                    missionList.Add(mission);
                }
            }
            else
            {
                HintEvent("WareHouseModel_StockIn has exception:shelfNo already exit");
            }

        }

        /// <summary>
        /// 出库
        /// </summary>
        /// <param name="shelfNo"></param>
        public void StockOut(string shelfNo)
        {
            try
            {
                if (!product_Info.ContainsKey(shelfNo))
                {
                    HintEvent(string.Format("WareHouseModel_StockOut has exception:shelfNo does not exit"));
                    return;
                }
                else
                {
                    mission = new Mission(MissionType.StockOut, shelfNo, modelPosition, product_Info, stackerParts_Info);
                    mission.MisSuccess += Mission_MisSuccess;
                    lock (obj_missionList)
                    {
                        missionList.Add(mission);
                    }
                }
            }
            catch (Exception ex)
            {
                HintEvent("WareHouseModel_StockOut has exception:" + ex.ToString());
            }
        }

        public void Sorting(string shelfNo)
        {
            if (!product_Info.ContainsKey(shelfNo))
            {
                HintEvent(string.Format("WareHouseModel_Sorting has exception:shelfNo_{0} does not exit", shelfNo));
                return;
            }
            else
            {
                mission = new Mission(MissionType.Sorting, shelfNo, modelPosition, product_Info, stackerParts_Info);
                mission.MisSuccess += Mission_MisSuccess;
                lock (obj_missionList)
                {
                    missionList.Add(mission);
                }
            }
        }

        private void Mission_MisSuccess(IMission mission, MissionType missionType, string shelfNo)
        {
            try
            {
                switch (missionType)//执行完是否需要重新拉取货物数据库
                {
                    case MissionType.StockIn:
                        Reset();
                        lock (obj_missionList)
                        {
                            missionList.Remove(mission);
                        }
                        break;
                    case MissionType.StockOut:
                        lock (obj_missionList)
                        {
                            missionList.Remove(mission);
                        }
                        product_Info.Remove(shelfNo);
                        break;

                }
            }
            catch(Exception ex)
            {
                HintEvent("WareHouseModel_Mission_MisSuccess has exception:" + ex.ToString());
            }
        }

        /// <summary>
        /// 通知 所有任务 输送带状态更改
        /// </summary>
        /// <param name="conveyorName"></param>
        /// <param name="isReady"></param>
        public void SetConveyor(string conveyorName, bool isReady)
        {
            try
            {
                foreach (var mission in missionList)
                {
                    mission.Update(conveyorName, isReady);
                }
            }
            catch (Exception ex)
            {
                HintEvent("WareHouseModel_SetConveyor has exception:" + ex.ToString());
            }
            
        }








        /// <summary>
        /// 时钟事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(UpdateLocation);
        }

        private void UpdateLocation()
        {
            foreach (ProductInfo productInfos in product_Info.Values)
            {
                productInfos.Model.Transform = new TranslateTransform3D() { OffsetX = productInfos.ProductOffSet.X, OffsetY = productInfos.ProductOffSet.Z, OffsetZ = -productInfos.ProductOffSet.Y };//坐标转换
            }
            foreach (StackerPartsInfo StackerPartsInfo in stackerParts_Info.Values)
            {
                var model = this.FindName(StackerPartsInfo.ModelName);
                if (model != null)
                {
                    ((ModelVisual3D)model).Content.Transform = new TranslateTransform3D() { OffsetX = StackerPartsInfo.StackerOffSet.X, OffsetY = StackerPartsInfo.StackerOffSet.Z, OffsetZ = -StackerPartsInfo.StackerOffSet.Y };
                }
            }
            foreach (ShelfInfo shelfInfo in shelf_Info.Values)
            {
                shelfInfo.Model.Content.Transform = new TranslateTransform3D() { OffsetX = shelfInfo.ShelfOffSet.X, OffsetY = shelfInfo.ShelfOffSet.Y, OffsetZ = shelfInfo.ShelfOffSet.Z };
            }
            if (missionList.Count != 0)
            {
                Point3D tempWorldPosition = ((Mission)missionList[missionList.Count - 1]).worldPosition;
                Point3D tempCameraPosition = Point3D.Add(tempWorldPosition, new Vector3D(3000, 2000, 2000));
                perspectiveCamera.Position = new Point3D(tempCameraPosition.X, tempCameraPosition.Z, -tempCameraPosition.Y);
                cameraLookDirection = new Vector3D(3000, 2000, -2000);
            }
        }

        public class WarehouseLocation
        {
            private int layerNo;//层
            private int rowNo;//行
            private int columnNo;//列
            private string location;

            public WarehouseLocation(int layer, int row, int column)
            {
                this.LayerNo = layer;
                this.RowNo = row;
                this.ColumnNo = column;
                this.location = layerNo + "," + rowNo + "," + columnNo;
            }

            public WarehouseLocation(string location)
            {
                this.location = location;
            }

            public string Location { get => location; set => location = value; }
            public int LayerNo { get => layerNo; set => layerNo = value; }
            public int RowNo { get => rowNo; set => rowNo = value; }
            public int ColumnNo { get => columnNo; set => columnNo = value; }
        }


    }





}


