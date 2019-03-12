using System;
using System.Collections;
using System.Collections.Generic;
using Dal;
using Entity.View;
using Entity.Model;
using Dal.Interface;
namespace Bll
{
    //UserInfo
    public class UserBll
    {
        private readonly IUserDal _service = new UserDal();

        #region  基础Method
        /// <summary>
        /// 添加一条数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public object Add(User model)
        {
            return _service.Insert(model);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool Update(User model)
        {
            return _service.Update(model) > 0;
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(object id)
        {
            return _service.Delete(id) > 0;
        }

        /// <summary>
        /// 逻辑删除数据 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool LogicDelete(User model)
        {
            return _service.LogicDelete(model) > 0;
        }

        /// <summary>
        /// 判断是否存在
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsExists(object id)
        {
            return _service.IsExists(id);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User FindEntity(object id)
        {
            return _service.FindEntity(id);
        }

        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <returns></returns>
        public List<User> FindList()
        {
            return _service.FindList();
        }

        /// <summary>
        /// 获取分页实体列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<User> PageList(int pageIndex, int pageSize, out int total)
        {
            return _service.PageList(pageIndex, pageSize, out total);
        }
        #endregion

        /// <summary>
        /// 获取用户权限等
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public UserInfoView GetAccessedControls(string account)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户账号获取用户信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public UserInfoView GetUserInfo(string account)
        {
            return _service.GetUserInfo(account);
        }

        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        public List<UserInfoView> GetUser()
        {
            return _service.GetUser();
        }
    }
}