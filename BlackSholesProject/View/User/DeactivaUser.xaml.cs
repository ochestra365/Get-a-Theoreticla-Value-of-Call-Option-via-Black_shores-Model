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
    public partial class DeactivaUser : Page//바꿀 부분
    {
        public DeactivaUser()//바꿀 부분
        {
            InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //업데이트할 떄 다 지우고 쓴다.

                //콤보박스 초기화
                List<string> comboValues = new List<string>()
                {
                    "False", "True"//0,1 인덱스값이 0과 1이라서 그렇다.
                };

                //그리드 바인딩 부분
                List<Model.User> users = Logic.DataAccess.GetUsesr();
                this.DataContext = users;
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
        
        private async void BtnAdd_Click(object sender, RoutedEventArgs e)//DB에서 고유키를 걸어줘서 더 이상 중복 데이터가 들어가지 않게 해주는 것이다. DB 디자인에서 UserEmail과 UserIdentityNumber에 대해 Unique키 설정. 스크립트 저장을 하고 나서 테이블 새로고침을 해야 나온다. D
        {
            bool isvalid = true;
            if (GrdData.SelectedItem == null)
            {
                await Commons.ShowMessageAsync("오류", "비활성화할 사용자를 선택하세요");//대기된 작업에서 이것이 시행됨
                return;
            }
            if (isvalid)
            {
                try
                {
                    var user = GrdData.SelectedItem as Model.User;
                    user.UserActivated = false;//사용자 비활성화

                    var result = Logic.DataAccess.SetUser(user);
                    if (result == 0)
                    {
                         await Commons.ShowMessageAsync("오류", "사용자 수정에 실패했어요");//대기된 작업에서 이것이 시행됨
                        return;
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

        private void GrdData_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                var user = GrdData.SelectedItem as Model.User;
            }
            catch (Exception ex)
            {
                Commons.LOGGER.Error($"예외발생 GrdData_SelectedCellsChanged : {ex}");
            }
        }
    }
}
