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
namespace WpfSMSApp.View.User
{
    /// <summary>
    /// MyAccountView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AddUser : Page//바꿀 부분
    {
        public AddUser()//바꿀 부분
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LblUserIdentityNumber.Visibility = LblUserSurName.Visibility = LblUserName.Visibility=LblUserEmail.Visibility=LblUserPassword.Visibility=LblUserAdmin.Visibility=LblUserActivated.Visibility= Visibility.Hidden;
                //업데이트할 떄 다 지우고 쓴다.

                //콤보박스 초기화
                List<string> comboValues = new List<string>()
                {
                    "False", "True"//0,1 인덱스값이 0과 1이라서 그렇다.
                };
                CboUserAdmin.ItemsSource = comboValues;
                CboUserActivated.ItemsSource = comboValues;

                TxtUserId.Text = TxtUserIdentityNumber.Text = "";
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 EditAccount Loaded: {ex}");
                throw ex;
            }
        }
        private void BtnUpdate_Click(object sender, RoutedEventArgs e)//네이밍 잘못줬다. 그래서 이전이 수정으로 바뀜 이거 고치로면 꼬임. 이게 새이벤트 처리기로 해야 하는데 기존에 있는 걸로 클릭해서 쓰니까 오류가 발생함.
        {
            NavigationService.GoBack();
        }
        public bool IsValidInput()
        {
            bool isvalid = true;

            if (string.IsNullOrEmpty(TxtUserIdentityNumber.Text))
            {
                LblUserIdentityNumber.Visibility = Visibility.Visible;
                LblUserIdentityNumber.Text = "사번을 입력하세요.";
                isvalid = false;
            }
            else
            {
                var cnt = Logic.DataAccess.GetUsesr().Where(u => u.UserIdentityNumber.Equals(TxtUserIdentityNumber.Text)).Count();//데이터베이스에서 검색하는 것이다.
                if (cnt > 0)
                {
                    LblUserIdentityNumber.Visibility = Visibility.Visible;//파라미터값을 잘못줘서 시스템이 이상한 네임을 찾는다.
                    LblUserIdentityNumber.Text = "중복된 사번이 존재합니다.";
                    isvalid = false;
                }
            }
            if (string.IsNullOrEmpty(TxtUserSurName.Text))
            {
                LblUserSurName.Visibility = Visibility.Visible;
                LblUserSurName.Text = "이름(성)이 빈값입니다.";
                isvalid = false;
            }
            if (string.IsNullOrEmpty(TxtUserName.Text))
            {
                LblUserName.Visibility = Visibility.Visible;
                LblUserName.Text = "이름빈값입니다.";
                isvalid = false;
            }
            if (string.IsNullOrEmpty(TxtUserSurName.Text))//여기 68번 코드라인이랑 중첩된다.
            {
                LblUserSurName.Visibility = Visibility.Visible;
                LblUserSurName.Text = "이름(성)이 빈값입니다.";
                isvalid = false;
            }
            if (string.IsNullOrEmpty(TxtUserEmail.Text))
            {
                LblUserEmail.Visibility = Visibility.Visible;
                LblUserEmail.Text = "메일이 빈값입니다.";
                isvalid = false;
            }

            if (!Commons.IsValidEmail(TxtUserEmail.Text))//검색해서 맞는 것의 반대를 찾는 조건이다.
            {
                LblUserEmail.Visibility = Visibility.Visible;
                LblUserEmail.Text = "이메일 형식이 올바르지 않습니다.";
                isvalid = false;
            }
            else
            {
                var cnt = Logic.DataAccess.GetUsesr().Where(u => u.UserEmail.Equals(TxtUserEmail.Text)).Count();
                if (cnt > 0)
                {
                    LblUserEmail.Visibility = Visibility.Visible;
                    LblUserEmail.Text = "중복된 이메일이 존재합니다.";
                    isvalid = false;
                }
            }
            if (string.IsNullOrEmpty(TxtUserPasword.Password))
            {
                LblUserPassword.Visibility = Visibility.Visible;
                LblUserPassword.Text = "패스워드 빈값입니다.";
                isvalid = false;
            }
            if (CboUserAdmin.SelectedIndex < 0)
            {
                LblUserAdmin.Visibility = Visibility.Visible;
                LblUserAdmin.Text = "관리자 여부를 선택하세요.";
                isvalid = false;
            }
            if (CboUserActivated.SelectedIndex < 0)
            {
                LblUserActivated.Visibility = Visibility.Visible;
                LblUserActivated.Text = "활성 여부를 선택하세요.";
                isvalid = false;
            }
            return isvalid;
        }
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            bool isvalid = true;
            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility = LblUserName.Visibility = LblUserEmail.Visibility = LblUserPassword.Visibility = LblUserAdmin.Visibility = LblUserActivated.Visibility = Visibility.Hidden;//화면에 뜬 값들을 숨겨준다.
            var user = new Model.User();// user는 DB모델의 사용자 목록들을 가져오겠따.
            isvalid= IsValidInput();
            if (isvalid)
            {
                //MessageBox.Show("DB수정처리!")
                user.UserIdentityNumber = TxtUserIdentityNumber.Text;
                user.UserSurname = TxtUserSurName.Text;
                user.UserName = TxtUserName.Text;
                user.UserEmail = TxtUserEmail.Text;
                user.UserPassword = TxtUserPasword.Password;
                user.UserAdmin = bool.Parse(CboUserAdmin.SelectedValue.ToString());//데이터 베이스의 관리자 컨트롤에서 선택된 값이 관리자가 맞는 지, 그리고 데이터타입을 문자열로 보냈는 지 분석을 하고 참이면 그 값을 가져와라.
                user.UserActivated = bool.Parse(CboUserActivated.SelectedValue.ToString());

                try
                {
                    var mdHash = MD5.Create();
                    user.UserPassword = Commons.GetMd5Hash(mdHash, user.UserPassword);//패스워드 암호화하는 것.

                    var result = Logic.DataAccess.SetUser(user);
                    if (result == 0)
                    {
                        //수정 안됨
                        LblResult.Text = "사용자 입력에 문제가 발생했습니다. 관리자에게 문의바랍니다.";
                        LblResult.Foreground = Brushes.OrangeRed;
                    }
                    else
                    {
                        //정상적 수정됨
                        NavigationService.Navigate(new UserList());
                    }
                }
                catch (Exception ex)//페이지는 메트로 인트로가 아니기 때문에 메시지 박스가 발생하지 않는다.
                {
                    Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }
    }
}
