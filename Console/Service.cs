using Newtonsoft.Json;
using NlpBridge;
using clientModel = Console.Models.Client;
using nlpModel = Console.Models.Nlp;


namespace Console
{
    public class Service : IService
    {
        private readonly IExecutor<clientModel.Person, clientModel.Credentials, nlpModel.Request.Request, nlpModel.Response.Response> _executor;

        public Service(IExecutor<clientModel.Person, clientModel.Credentials, nlpModel.Request.Request, nlpModel.Response.Response> executor)
        {
            this._executor = executor;
        }

        public async Task RunAsync()
        {
            var request = new clientModel.Person
            {
                Name = "Edwin",
                Email = "edwin@gmail.com",
                PhoneNumber = "1234567890"
            };
            var prompt = "Build a unique username and password using the provided information below";
            var response = await _executor.ExecuteAsync(request, prompt);
            System.Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}
