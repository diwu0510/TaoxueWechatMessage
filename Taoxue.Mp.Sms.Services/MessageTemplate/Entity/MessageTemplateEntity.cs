using HZC.Database;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 消息模板
    /// </summary>
    [MyDataTable("Message_MessageTemplate")]
    public class MessageTemplateEntity : BaseEntity
    {
        /// <summary>
        /// 模板ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 模板内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 第三方平台ID
        /// </summary>
        public int PlatId { get; set; }

        /// <summary>
        /// 是否有效
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public bool Enabled { get; set; } = false;
    }
}
