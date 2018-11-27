namespace HZC.Utils.UEditor
{
    public class UEditorConfig
    {
        public UEditorConfig()
        { }

        /* 上传图片配置项 */
        public string imageActionName { get; set; }
        public string imageFieldName { get; set; }
        public long imageMaxSize { get; set; }
        public string[] imageAllowFiles { get; set; }
        public bool imageCompressEnable { get; set; }
        public int imageCompressBorder { get; set; }
        public string imageInsertAlign { get; set; }
        public string imageUrlPrefix { get; set; }
        public string imagePathFormat { get; set; }

        /* 涂鸦图片上传配置项 */
        public string scrawlActionName { get; set; }
        public string scrawlFieldName { get; set; }
        public string scrawlPathFormat { get; set; }
        public long scrawlMaxSize { get; set; }
        public string scrawlUrlPrefix { get; set; }
        public string scrawlInsertAlign { get; set; }

        /* 截图工具上传 */
        public string snapscreenActionName { get; set; }
        public string snapscreenPathFormat { get; set; }
        public string snapscreenUrlPrefix { get; set; }
        public string snapscreenInsertAlign { get; set; }

        /* 抓取远程图片配置 */
        public string[] catcherLocalDomain { get; set; }
        public string catcherActionName { get; set; }
        public string catcherFieldName { get; set; }
        public string catcherPathFormat { get; set; }
        public string catcherUrlPrefix { get; set; }
        public long catcherMaxSize { get; set; }
        public string[] catcherAllowFiles { get; set; }

        /* 上传视频配置 */
        public string videoActionName { get; set; }
        public string videoFieldName { get; set; }
        public string videoPathFormat { get; set; }
        public string videoUrlPrefix { get; set; }
        public long videoMaxSize { get; set; }
        public string[] videoAllowFiles { get; set; }

        /* 上传文件配置 */
        public string fileActionName { get; set; }
        public string fileFieldName { get; set; }
        public string filePathFormat { get; set; }
        public string fileUrlPrefix { get; set; }
        public long fileMaxSize { get; set; }
        public string[] fileAllowFiles { get; set; }

        /* 列出指定目录下的图片 */
        public string imageManagerActionName { get; set; }
        public string imageManagerListPath { get; set; }
        public int imageManagerListSize { get; set; }
        public string imageManagerUrlPrefix { get; set; }
        public string imageManagerInsertAlign { get; set; }
        public string[] imageManagerAllowFiles { get; set; }

        /* 列出指定目录下的文件 */
        public string fileManagerActionName { get; set; }
        public string fileManagerListPath { get; set; }
        public string fileManagerUrlPrefix { get; set; }
        public int fileManagerListSize { get; set; }
        public string[] fileManagerAllowFiles { get; set; }
    }
}
