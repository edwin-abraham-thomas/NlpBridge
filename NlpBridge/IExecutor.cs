﻿namespace NlpBridge
{
    public interface IExecutor<TClientRequest, TClientResponse, TNlpRequest, TNlpResponse>
        where TClientRequest : new()
        where TClientResponse : new()
        where TNlpRequest : new()
        where TNlpResponse : new()
    {
        public Task<TClientResponse> ExecuteAsync(TClientRequest request);
    }
}