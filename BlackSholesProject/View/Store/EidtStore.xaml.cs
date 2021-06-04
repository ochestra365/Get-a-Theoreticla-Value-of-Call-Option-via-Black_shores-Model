using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using MahApps.Metro.Controls.Dialogs;
namespace WpfSMSApp.View.Store
{
    /// <summary>
    /// MyAccountView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EidtStore : Page//바꿀 부분
    {
        private int StoreID { get; set; }
        //수정할 창고객체
        private Model.Store CurrentStore { get; set; }//데이터 베이스의 값을 그대로 가져온다. 전역변수로써 영향을 끼치고 있으니 각 메서드마다 따로 Model에 대한 식별자를 지정해줄 필요가 없어졌다. 그래서 코딩량이 줄어들게 되었다.
        public EidtStore()//바꿀 부분
        {
            InitializeComponent();
        }
        /// <summary>
        /// 추가생성자. StoreList에서 storeId를 받아옴
        /// </summary>
        /// <param name="storeId"></param>
        public EidtStore(int storeId) : this()
        {
            StoreID = storeId;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
                LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;//레이블을 숨기고
                TxtStoreId.Text = TxtStoreName.Text=TxtStoreLocation.Text = "";//텍스트를 빈값으로 초기화해준다. 오류가 발생하지 않는 파트이다.
            try
            {
                //Store테이블에서 내용 읽음.
                CurrentStore = Logic.DataAccess.GetStores().Where(s => s.StoreID.Equals(StoreID)).FirstOrDefault();//First로 하게 되면 값이 없을 때, 오류가 발생하게 됨.
                TxtStoreId.Text = CurrentStore.StoreID.ToString();
                TxtStoreName.Text = CurrentStore.StoreName;
                TxtStoreLocation.Text = CurrentStore.StoreLocation;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"EditStore.xama.cs Page_Loaded 예외발생 : {ex}");
                Commons.ShowMessageAsync("예외","예외발생");
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)//네이밍 잘못줬다. 그래서 이전이 수정으로 바뀜 이거 고치로면 꼬임. 이게 새이벤트 처리기로 해야 하는데 기존에 있는 걸로 클릭해서 쓰니까 오류가 발생함.
        {
            NavigationService.GoBack();
        }
        public bool IsValidInput()
        {
            bool isvalid = true;

            if (string.IsNullOrEmpty(TxtStoreName.Text))
            {
                LblStoreName.Visibility = Visibility.Visible;
                LblStoreName.Text = "창고명을 입력하세요.";
                isvalid = false;
            }
            else
            {
                var cnt = Logic.DataAccess.GetStores().Where(u => u.StoreName.Equals(TxtStoreName.Text)).Count();//데이터베이스에서 검색하는 것이다.
                if (cnt > 0)
                {
                    LblStoreName.Visibility = Visibility.Visible;//파라미터값을 잘못줘서 시스템이 이상한 네임을 찾는다.
                    LblStoreName.Text = "중복된 창고명이 존재합니다.";
                    isvalid = false;
                }
            }
            if (string.IsNullOrEmpty(TxtStoreLocation.Text))
            {
                LblStoreLocation.Visibility = Visibility.Visible;
                LblStoreLocation.Text = "창고위치를 입력하세요";
                isvalid = false;
            }
            return isvalid;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isvalid = true;//지역변수-->전역변수
            LblStoreName.Visibility = LblStoreLocation.Visibility = Visibility.Hidden;//화면에 뜬 값들을 숨겨준다.
            isvalid= IsValidInput();//유효성 체크-->개발자에게 필수이다.-->어디에서든 다 필요적인 것이다.
            if (isvalid)
            {
                //MessageBox.Show("DB수정처리!")
                CurrentStore.StoreName = TxtStoreName.Text;
                CurrentStore.StoreLocation = TxtStoreLocation.Text;
                try
                {
                    var result = Logic.DataAccess.SetStore(CurrentStore);
                    if (result == 0)
                    {
                        //수정 안됨
                        Commons.LOGGER.Error("AddStore.xaml.cs 창고정보 저장오류 발생");
                        Commons.ShowMessageAsync("오류", "저장 시 오류가 발생했습니다.");
                        return;
                    }
                    else
                    {
                        //정상적 수정됨
                      NavigationService.Navigate(new StoreList());
                    }
                }
                catch (Exception ex)//페이지는 메트로 인트로가 아니기 때문에 메시지 박스가 발생하지 않는다.
                {
                    Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }

        private void TxtStoreName_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }

        private void TxtStoreLocation_LostFocus(object sender, RoutedEventArgs e)
        {
            IsValidInput();
        }
    }
}
