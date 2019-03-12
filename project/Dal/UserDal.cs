using System.Collections.Generic;
using Dal.Interface;
using Entity.View;
using Entity.Model;

namespace Dal
{
    //UserInfo
    public class UserDal : Dal<User>,IUserDal
    {
        public UserInfoView GetUserInfo(string account)
        {
            string sql = "select * from userinfo Where UserCount = @0";
            return DbProvider.FirstOrDefault<UserInfoView>(sql, account);
        }

        public List<UserInfoView> GetUser()
        {
            string sql = "select Id,Account,Name,Status from userinfo";
            return DbProvider.Fetch<UserInfoView>(sql);
        }
    }
}