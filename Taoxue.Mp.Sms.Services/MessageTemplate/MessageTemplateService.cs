using HZC.Core;
using HZC.Database;
using System;
using System.Collections.Generic;

namespace Taoxue.Mp.Sms.Services
{
    public class MessageTemplateService
    {
        private MyDbUtil db;

        public MessageTemplateService(string sectionName = "")
        {
            db = new MyDbUtil(sectionName);
        }

        /// <summary>
        /// 创建模板
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Create(MessageTemplateEntity entity, AppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name) || string.IsNullOrWhiteSpace(entity.TemplateId))
            {
                return ResultUtil.AuthFail("模板名称或模板ID不能为空");
            }
            entity.Enabled = true;
            entity.BeforeCreate(user);
            var id = db.Create<MessageTemplateEntity>(entity);
            if (id > 0)
            {
                MessageTemplateUtil.Clear();
                return ResultUtil.Success<int>(id);
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 更新模板
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Update(MessageTemplateEntity entity, AppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name) || string.IsNullOrWhiteSpace(entity.TemplateId))
            {
                return ResultUtil.AuthFail("模板名称或模板ID不能为空");
            }
            entity.BeforeUpdate(user);
            var row = db.Update<MessageTemplateEntity>(entity);
            if (row > 0)
            {
                MessageTemplateUtil.Clear();
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <param name="entity">模板实体</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Toggle(MessageTemplateEntity entity, AppUser user)
        {
            var row = db.Update<MessageTemplateEntity>(
                KeyValuePairs.New()
                    .Add("Enabled", !entity.Enabled)
                    .Add("Updator", user.Name).Add("UpdateAt", DateTime.Now),
                MySearchUtil.New().AndEqual("Id", entity.Id));
            if (row > 0)
            {
                MessageTemplateUtil.Clear();
                return ResultUtil.Success();
            }
            else
            {
                return ResultUtil.Fail();
            }
        }

        /// <summary>
        /// 所有模板
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <returns></returns>
        public IEnumerable<MessageTemplateEntity> Fetch(MessageTemplateSearchParam param)
        {
            return db.Fetch<MessageTemplateEntity>(param.ToSearchUtil());
        }

        /// <summary>
        /// 加载分页列表
        /// </summary>
        /// <param name="param">查询参数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns></returns>
        public PageList<MessageTemplateEntity> Query(MessageTemplateSearchParam param, int pageIndex, int pageSize)
        {
            return db.Query<MessageTemplateEntity>(param.ToSearchUtil(), pageIndex, pageSize);
        }
    }
}
