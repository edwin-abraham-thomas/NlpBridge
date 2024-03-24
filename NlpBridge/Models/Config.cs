using System.Linq.Expressions;

namespace NlpBridge.Models
{
    public class Config<TRequest>
    {
        public required string NlpServiceUrl { get; set; }
        public IDictionary<string, string>? NlpUrlParams { get; set; }
        public required Expression<Func<TRequest, string>> PromptProperty { get; set; }
    }
}
