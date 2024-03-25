using System.Linq.Expressions;

namespace NlpBridge.Models
{
    public class Config<TNlpRequest, TNlpResponse>
    {
        public required string NlpServiceUrl { get; set; }
        public IDictionary<string, string>? NlpUrlParams { get; set; }
        public required Expression<Func<TNlpRequest, string>> PromptProperty { get; set; }
        public required Expression<Func<TNlpResponse, string>> ResponseTextProperty { get; set; }
        public required TNlpRequest DefaultRequest { get; set; }
    }
}
