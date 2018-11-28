using HZC.Core;
using HZC.Database;
using HZC.Utils;
using System;
using System.Collections.Generic;

namespace Taoxue.Mp.Sms.Services
{
    public class PlatService
    {
        private MyDbUtil db;
        public PlatService(string sectionName = "")
        {
            db = new MyDbUtil(sectionName);
        }
        
        /// <summary>
        /// 创建第三方平台
        /// </summary>
        /// <param name="entity">平台实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Create(PlatEntity entity, AppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return ResultUtil.AuthFail("平台名称不能为空");
            }

            string random = new Random().Next(10000, 99999).ToString();
            entity.SecretKey = AESEncryptUtil.Encrypt(random);
            entity.BeforeCreate(user);
            var id = db.Create<PlatEntity>(entity);
            if (id > 0)
            {
                PlatUtil.Clear();
                return ResultUtil.Success<int>(id);
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 更新第三方平台
        /// </summary>
        /// <param name="entity">平台实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Update(PlatEntity entity, AppUser user)
        {
            entity.BeforeUpdate(user);
            var row = db.Update<PlatEntity>(entity);
            if (row > 0)
            {
                PlatUtil.Clear();
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 禁用第三方平台
        /// </summary>
        /// <param name="entity">平台实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Toggle(PlatEntity entity, AppUser user)
        {
            var row = db.Update<PlatEntity>(
                KeyValuePairs.New()
                    .Add("Enabled", !entity.Enabled)
                    .Add("Updator", user.Name)
                    .Add("UpdateAt", DateTime.Now),
                MySearchUtil.New().AndEqual("Id", entity.Id));
            if (row > 0)
            {
                PlatUtil.Clear();
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 重置SecretKey
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result ResetSecretKey(PlatEntity entity, AppUser user)
        {
            entity.SecretKey = GetSecretKey();
            var row = db.Update<PlatEntity>(
                KeyValuePairs.New()
                    .Add("SecretKey", GetSecretKey())
                    .Add("UpdateAt", DateTime.Now)
                    .Add("Updator", user.Name),
                MySearchUtil.New().AndEqual("Id", entity.Id));
            if (row > 0)
            {
                PlatUtil.Clear();
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 加载分页列表
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        public PageList<PlatEntity> Query(PlatSearchParam param, int pageIndex, int pageSize)
        {
            var pl = db.Query<PlatEntity>(param.ToSearchUtil(), pageIndex, pageSize);
            return pl;
        }

        /// <summary>
        /// 所有平台
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PlatEntity> Fetch()
        {
            return db.Fetch<PlatEntity>(MySearchUtil.New());
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PlatEntity Load(int id)
        {
            return db.Load<PlatEntity>(id);
        }

        /// <summary>
        /// 获取SecretKey
        /// </summary>
        /// <returns></returns>
        private string GetSecretKey()
        {
            string random = new Random().Next(10000, 99999).ToString();
            return AESEncryptUtil.Encrypt(random);
        }
    }
}
