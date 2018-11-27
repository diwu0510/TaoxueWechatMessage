using HZC.Database;

namespace Taoxue.Mp.Sms.Services
{
    /// <summary>
    /// 第三方平台
    /// </summary>
    [MyDataTable("Base_Plate")]
    public class PlatEntity : BaseEntity
    {
        /// <summary>
        /// 第三方平台名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 第三方平台秘钥
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public string SecretKey { get; set; }

        /// <summary>
        /// 是否允许接入
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public bool Enabled { get; set; } = true;
    }
}
