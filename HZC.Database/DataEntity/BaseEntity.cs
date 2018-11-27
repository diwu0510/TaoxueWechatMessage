using HZC.Core;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace HZC.Database
{
    /// <summary>
    /// 所有数据实体的基类
    /// </summary>
    public class BaseEntity : BaseEntity<int>
    {
        [MyDataField(UpdateIgnore = true)]
        public bool IsDel { get; set; }

        //[Timestamp]
        //public byte[] Version { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime CreateAt { get; set; }

        /// <summary>
        /// 创建人ID
        /// </summary>
        //[MyDataField(UpdateIgnore = true)]
        //public int CreateBy { get; set; }

        /// <summary>
        /// 创建人帐号
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public string Creator { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime UpdateAt { get; set; }

        /// <summary>
        /// 更新人ID
        /// </summary>
        //public int UpdateBy { get; set; }

        /// <summary>
        /// 更新人帐号
        /// </summary>
        public string Updator { get; set; }

        /// <summary>
        /// 插入前设置跟踪项
        /// </summary>
        /// <param name="user"></param>
        public void BeforeCreate(AppUser user)
        {
            CreateAt = DateTime.Now;
            //CreateBy = user.Id;
            Creator = user.Name;
            UpdateAt = DateTime.Now;
            //UpdateBy = user.Id;
            Updator = user.Name;
        }

        /// <summary>
        /// 编辑前设置跟踪项
        /// </summary>
        /// <param name="user"></param>
        public void BeforeUpdate(AppUser user)
        {
            UpdateAt = DateTime.Now;
            //UpdateBy = user.Id;
            Updator = user.Name;
        }
    }
}
