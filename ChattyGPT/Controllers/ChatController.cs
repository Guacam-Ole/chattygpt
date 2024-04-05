using Microsoft.AspNetCore.Mvc;

namespace ChattyGPT.Controllers
{
    [ApiController]
    [Route("/")]
    public class ChatController : ControllerBase
    {
        [HttpPost("/ask/{id}")]
        public async Task<string> AskStuff([FromRoute] string id, [FromBody] string question)
        {
            var ai = new FancyAi(id);
            return await ai.AskSomething(question) ?? "oopsie. Didn't work";
        }

        [HttpGet("/ids")]
        public List<string> GetIds()
        {
            return FancyAi.GetAllBotIds();
        }

        [HttpGet("/")]
        public ActionResult Render()
        {
            return base.Content(System.IO.File.ReadAllText("ask.html"), "text/html");
        }
    }
}