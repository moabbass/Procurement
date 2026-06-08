using GenerativeAI;
using GenerativeAI.Types;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Procurement.Components.Model
{
    public class PDFValidatorService
    {
        private string _apiKey;

        public async Task<(bool IsViable, string Reason)> CheckPdfViabilityAsync(byte[] pdfBytes, string contextType, IConfiguration config)
        {
            _apiKey = config["Gemini:ApiKey"]
                      ?? throw new Exception("Gemini API Key is missing in configuration.");
            //  Initialize the model
            var client = new GenerativeModel(model: "gemini-2.5-flash", apiKey: _apiKey);

            //  Define the strict prompt
            string prompt = $@"
                Analyze this document. It is intended to be a {contextType} in a procurement system.
                Check for the following:
                1. Does it contain professional language?
                2. Is there a clear entity/company name?
                3. Are there specific requirements or pricing mentioned?
                4. Is it a relevant business document (not a photo or a blank page)?

                Return only a JSON object: 
                {{ ""viable"": true, ""reason"": ""Brief explanation"" }}";

            //  Create the multi-part content

            // Use Convert.ToBase64String for the Data property
            var parts = new List<Part>
            {
                new Part { Text = prompt },
                new Part {
                    InlineData = new Blob {
                        MimeType = "application/pdf",
                        Data = Convert.ToBase64String(pdfBytes)
                    }
                }
            };

            try
            {
                var response = await client.GenerateContentAsync(parts);

                // Clean and Parse JSON
                string jsonString = response.Text.Replace("```json", "").Replace("```", "").Trim();

                var result = JsonSerializer.Deserialize<AiResponse>(jsonString);
                return (result?.viable ?? false, result?.reason ?? "Unknown error during AI analysis");
            }
            catch (Exception ex)
            {
                return (false, $"AI validation failed: {ex.Message}");
            }
        }
    }

    public class AiResponse
    {
        public bool viable { get; set; }
        public string reason { get; set; }
    }
}