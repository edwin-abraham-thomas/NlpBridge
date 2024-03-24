using Console.Models.Request;
using Console.Models.Response;
using Newtonsoft.Json;
using NlpBridge;

namespace Console
{
    public class Service : IService
    {
        private readonly IExecutor<Request, Response> _executor;

        public Service(IExecutor<Request, Response> executor)
        {
            this._executor = executor;
        }

        public async Task RunAsync()
        {
            var request = new Request
            {
                Contents = new List<Models.Request.Content>
                {
                    new Models.Request.Content
                    {
                        Parts = new List<Models.Request.Part> 
                        {
                            new Models.Request.Part
                            {
                                Text = "Hi, how are you"
                            }
                        }
                    }
                }
            };

            var response = await _executor.ExecuteAsync(request);
            System.Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
