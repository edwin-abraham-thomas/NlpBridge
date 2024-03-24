namespace NlpBridge
{
    public interface IExecutor<TRequest, TResponse> where TRequest : new() where TResponse : new()
    {
        public Task<TResponse> ExecuteAsync(TRequest request);
    }
}