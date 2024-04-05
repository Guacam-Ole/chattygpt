using Newtonsoft.Json;

using OpenAI;
using OpenAI.Interfaces;
using OpenAI.Managers;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.ObjectModels;
using Microsoft.AspNetCore.Server.HttpSys;

namespace ChattyGPT
{
    public class FancyAi
    {
        private Bot _bot;

        public FancyAi(string id)
        {
            var allBots = JsonConvert.DeserializeObject<List<Bot>>(File.ReadAllText("bots.json"));
            _bot = allBots.FirstOrDefault(q => q.Name == id) ?? throw new Exception("id is missing");
        }

        public static List<string> GetAllBotIds()
        {
            var allBots = JsonConvert.DeserializeObject<List<Bot>>(File.ReadAllText("bots.json"));
            return allBots.Select(q => q.Name).ToList();
        }

        public async Task<string?> AskSomething(string prompt)
        {
            var service = Login();
            return await GetResponse(service, prompt);    
        }

        private async Task<string?> GetResponse(OpenAIService openAiService, string userPrompt)
        {
            var messages=new List<ChatMessage>();
            foreach (var botPrompt in _bot.Prompts)
            {
                messages.Add(ChatMessage.FromSystem(botPrompt));
            }
            messages.Add(ChatMessage.FromUser(userPrompt));

            var result = await openAiService.ChatCompletion.CreateCompletion(new ChatCompletionCreateRequest
            {
                Messages=messages,
                Model = Models.Gpt_3_5_Turbo,
                Temperature = 0.9f,
                MaxTokens = 250 //optional
            });

            if (result.Successful)
            {
                return result.Choices.First().Message.Content;
            }
            return null;
        }

        

        private static OpenAIService Login()
        {
            var secrets = JsonConvert.DeserializeObject<Secrets>(File.ReadAllText("secrets.json")) ?? throw new Exception ("cannot read secrets");
            var openAiService = new OpenAIService(new OpenAiOptions()
            {
                ApiKey = secrets.OpenAiKey
            });
            return openAiService;
        }
    }
}
