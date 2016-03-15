namespace Saturn72.Core.Net.Rest
{
    public sealed class RestResponse
    {
        public string Content { get; set; }
        public string ContentEncoding { get; set; }
        public long ContentLength { get; set; }
        public string ContentType { get; set; }
    }
}