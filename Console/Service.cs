using Newtonsoft.Json;
using NlpBridge;
using clientModel = Console.Models.Client;
using nlpModel = Console.Models.Nlp;


namespace Console
{
    public class Service : IService
    {
        private readonly IExecutor<clientModel.Request, clientModel.Response, nlpModel.Request.Request, nlpModel.Response.Response> _executor;

        public Service(IExecutor<clientModel.Request, clientModel.Response, nlpModel.Request.Request, nlpModel.Response.Response> executor)
        {
            this._executor = executor;
        }

        public async Task RunAsync()
        {
            var request = new clientModel.Request
            {
                Name = "Edwin",
                Email = "edwin@gmail.com",
                PhoneNumber = "1234567890"
            };

            var response = await _executor.ExecuteAsync(request);
            System.Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
