using System;
using System.Collections.Generic;
using System.Data;
using Entity.View;
using Entity.Model;
namespace Dal.Interface
{
	/// <summary>
	/// 接口层User
	/// </summary>
	public interface IUserDal: IDal<User>
	{
	    UserInfoView GetUserInfo(string account);
	    List<UserInfoView> GetUser();
	} 
}