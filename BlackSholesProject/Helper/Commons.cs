using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using BlackSholesProject.Model;

namespace WpfSMSApp
{
    public class Commons
    {
        //NLog 정적 인스턴스 생성
        public static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();

        // 로그인한 유저 정보
        public static User LOGINED_USER;

        //암호화 해줘서 관리자도 볼 수 없게 하는 것이다.
        /// <summary>
        /// mt5 암호화 처리 메서드
        /// </summary>
        /// <param name="md5Hash"></param>
        /// <param name="plainStr"></param>
        /// <returns></returns>
        public static string GetMd5Hash(MD5 md5Hash, string plainStr)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(plainStr));//using System.Security.Cryptography;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                builder.Append(data[i].ToString("x2"));//모든 글자를 16진수로 바꾼다.
            }

            return builder.ToString();
        }
        /// <summary>
        /// 이메일 정규식 확인 메서드
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        internal static bool IsValidEmail(string email)//using System.Text.RegularExpressions; 정규식
        {
            return Regex.IsMatch(email, @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?");
        }
        /// <summary>
        /// Metro MessageBox 공통메서드
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public static async Task<MessageDialogResult> ShowMessageAsync(string title, string message, MessageDialogStyle style = MessageDialogStyle.Affirmative)//비동기로 일처리된 내용이 메세지처럼 돌아온 다는 것을 의미한다.
        {
            //this랑 같은 것이다.
            return await ((MetroWindow)Application.Current.MainWindow).ShowMessageAsync(title, message, style, null);//매개변수만 설정해주려고 데이터타입을 string 준 것이다. 해당 메서드를 사용하면 이 메서드의 값을 불러 내겠다. 단 윈도우와 동기화를 풀어서 보여주겠다. 현재 윈도우화면의 상태를 의미한다.
        }
    }
}
