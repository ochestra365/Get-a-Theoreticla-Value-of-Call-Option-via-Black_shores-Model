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
namespace WpfSMSApp.Account
{
    /// <summary>
    /// MyAccountView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class EditAccount : Page
    {
        public EditAccount()
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


                var user = Commons.LOGINED_USER;//데이터베이스에서 값들을 가져오는 것이다.
                TxtUserId.Text = user.UserID.ToString();//해당 값들을 여기 툴에서 문자데이터타입으로 옮겨주는 것을 의미한다.
                TxtUserIdentityNumber.Text = user.UserIdentityNumber.ToString();
                TxtUserSurName.Text = user.UserSurname.ToString();
                TxtUserEmail.Text = user.UserEmail.ToString();
                TxtUserName.Text = user.UserName.ToString();
                //TxtUserPasword.Password = user.UserPassword; 오류가 발생하기 때문에 제거함.
                CboUserAdmin.SelectedIndex = user.UserAdmin == false ? 0 : 1;
                CboUserActivated.SelectedIndex = user.UserActivated == false ? 0 : 1;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 EditAccount Loaded: {ex}");
                throw ex;
            }
        }

        private void BtnEditMyAccount_Click(object sender, RoutedEventArgs e)//이거 네이밍 안돼어 있다. 새 명령 편집기를 xaml 이벤트 생성창에서 열어주는 것이 가장 정확하다.
        {
            bool isvalid = true;
            LblUserIdentityNumber.Visibility = LblUserSurName.Visibility = LblUserName.Visibility = LblUserEmail.Visibility = LblUserPassword.Visibility = LblUserAdmin.Visibility = LblUserActivated.Visibility = Visibility.Hidden;
            var user = Commons.LOGINED_USER;

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
            if (string.IsNullOrEmpty(TxtUserPasword.Password))
            {
                LblUserPassword.Visibility = Visibility.Visible;
                LblUserPassword.Text = "패스워드 빈값입니다.";
                isvalid = false;
            }
            if (isvalid)
            {
                //MessageBox.Show("DB수정처리!")
                user.UserSurname = TxtUserSurName.Text;
                user.UserName = TxtUserName.Text;
                user.UserEmail = TxtUserEmail.Text;
                user.UserPassword = TxtUserPasword.Password;
                user.UserAdmin = bool.Parse(CboUserAdmin.SelectedValue.ToString());
                user.UserActivated = bool.Parse(CboUserActivated.SelectedValue.ToString());

                try
                {
                    var mdHash = MD5.Create();
                    user.UserPassword = Commons.GetMd5Hash(mdHash, user.UserPassword);//패스워드 암호화하는 것.

                    var result = Logic.DataAccess.SetUser(user);
                    if (result == 0)
                    {
                        //수정 안됨
                        LblResult.Text = "계정 수정에 문제가 발생했습니다. 관리자에게 문의바랍니다.";
                        LblResult.Foreground = Brushes.OrangeRed;
                    }
                    else
                    {
                        //정상적 수정됨
                        LblResult.Text = "정상적으로 수정했습니다.";
                        LblResult.Foreground = Brushes.DeepSkyBlue;
                    }
                }
                catch (Exception ex)//페이지는 메트로 인트로가 아니기 때문에 메시지 박스가 발생하지 않는다.
                {
                    Commons.LOGGER.Error($"예외발생 : {ex}");
                }
            }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)//네이밍 잘못줬다. 그래서 이전이 수정으로 바뀜 이거 고치로면 꼬임. 이게 새이벤트 처리기로 해야 하는데 기존에 있는 걸로 클릭해서 쓰니까 오류가 발생함.
        {
            NavigationService.GoBack();
        }
    }
}
