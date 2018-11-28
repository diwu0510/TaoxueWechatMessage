using HZC.Core;
using HZC.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Taoxue.Mp.Sms.Services
{
    public class OMessageService
    {
        private readonly MyDbUtil db;

        public OMessageService(string sectionName)
        {
            db = new MyDbUtil(sectionName);
        }

        public Result Create(OMessageEntity entity)
        {
            // 验证来源
            var plat = PlatUtil.Get(entity.PlateId);
            if (plat == null || !plat.Enabled)
            {
                return ResultUtil.AuthFail("平台不存在或已禁用");
            }

            // 验证模板
            var temp = MessageTemplateUtil.Get(entity.TemplateId);
            if (temp == null || !temp.Enabled)
            {
                return ResultUtil.AuthFail("模板不存在或已禁用");
            }

            

            var id = db.Create<OMessageEntity>(entity);
            return id > 0 ? ResultUtil.Success<int>(id) : ResultUtil.Fail();
        }
    }
}
