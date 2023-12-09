using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;

using OpenAI_API;
using OpenAI_API.Completions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IrisWeb.Controllers.WebApi
{
    public class Prompt
    {
        public string UserPrompt { get; set; }
        public string TextToRefine { get; set; }
    }



    [Route("api/[controller]")]
    [ApiController]
    public class ChatGPTController : ControllerBase
    {
        [HttpPost("useChatGPT")]
   
        public async Task<IActionResult> useChatGPT([FromBody] Prompt query)
        {
            string outputResult = "";
            string refinedText = query.TextToRefine.Replace(":\n\n", "\n");
            // Initialize conversation history with user prompt and text to refine
            List<string> conversationHistory = new List<string>
                {
                    $"User: {query.UserPrompt}",
                    $"Refinement Text: {refinedText}"
                };

            var openAi = new OpenAIAPI("YOUR-OPENAI-API-KEY");
            List<string> modelResponses = new List<string>(); // Store model responses separately

            // Assuming there's only one turn for user input and refinement text
            foreach (var turn in conversationHistory)
            {
                var completionRequest = new CompletionRequest
                {
                    Prompt = string.Join("\n", conversationHistory),
                    Model = OpenAI_API.Models.Model.DavinciText,
                    MaxTokens = 3000,
                     Temperature = 0.2
                };

                try
                {
                    var completions = await openAi.Completions.CreateCompletionAsync(completionRequest);

                    if (completions?.Completions != null && completions.Completions.Any())
                    {
                        outputResult = completions.Completions.Last().Text;
                        modelResponses.Add($"Model: {outputResult}");
                    }
                    else
                    {
                        // Handle empty or null responses
                        // Log or take appropriate action
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions
                    // Log or take appropriate action
                }
            }

            // Update the conversation history with the model's responses
            conversationHistory.AddRange(modelResponses);

            var json = JsonConvert.SerializeObject(new
            {
                data = outputResult
            });

            return Ok(json);


        }
    }
}
