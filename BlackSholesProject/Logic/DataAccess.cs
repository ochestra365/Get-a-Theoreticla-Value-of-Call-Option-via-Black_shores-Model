using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfSMSApp.Model;

namespace WpfSMSApp.Logic
{
    public class DataAccess
    {
        public static List<User> GetUsesr()
        {
            List<User> users;

            using(var ctx = new SMSEntities())
            {
                users = ctx.User.ToList();//SELECT*FROM user
            }
            return users;
        }
        /// <summary>
        /// 입력과 수정을 동시에 하는 것이다.
        /// </summary>
        /// <param name="user"></param>
        /// <returns>0또는 1이상</returns>
        internal static int SetUser(User user)
        {
            using (var ctx = new SMSEntities())
            {
                ctx.User.AddOrUpdate(user);
                return ctx.SaveChanges();//Commit
            }
        }

        internal static List<Store> GetStores()
        {
            List<Store> stores;
            using(var ctx = new SMSEntities())
            {
                stores = ctx.Store.ToList();
            }
            return stores;
        }

        public static List<Stock> GetStocks()//복붙했는데 자꾸 클래스 명을 같이 써버리는 실수를 한다.
        {
            List<Stock> stocks;
            using (var ctx = new SMSEntities())
            {
                stocks = ctx.Stock.ToList();
            }
            return stocks;
        }

        internal static int SetStore(Store store)
        {
            using(var ctx =new SMSEntities())
            {
                ctx.Store.AddOrUpdate(store);
                return ctx.SaveChanges();//commit
            }
        }
    }
}
// entity 프레임워크를 사용하면 쿼리 작성을 하지 않고도 손 쉽게 DB와 연동할 수 있다. 코딩량이 줄어든다.
//ctx는 Context의 약자이다.