namespace HZC.Database
{
    /// <summary>
    /// 数据实体的基类
    /// </summary>
    /// <typeparam name="TKeyType">主键的数据类型，当前只支持int类型</typeparam>
    public class BaseEntity<TKeyType>
    {
        [MyKey]
        public TKeyType Id { get; set; }
    }
}
