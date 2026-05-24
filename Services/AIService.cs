using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Modesta.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Modesta.Services
{
    public class AIService
    {
        private static string _connString = @"Data Source=C:\Users\Heidi\modesta.db";
        private static string _apiKey = "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";
        private readonly HttpClient _httpClient = new HttpClient();

        private Modesta.Data.ModestDbContext GetDb()
        {
            var options = new DbContextOptionsBuilder<Modesta.Data.ModestDbContext>()
                .UseSqlite(_connString)
                .Options;
            return new Modesta.Data.ModestDbContext(options);
        }

        public async Task<string> GetOutfitSuggestion(int userId, string question)
        {
            using (var db = GetDb())
            {
                var items = db.ClothingItems
                    .Where(i => i.UserId == userId)
                    .ToList();

                var kastBeschrijving = string.Join(", ",
                    items.Select(i => $"{i.Name} ({i.Color}, {i.Category})"));

                if (string.IsNullOrEmpty(kastBeschrijving))
                    kastBeschrijving = "lege kledingkast";

                var prompt = $"Mijn kledingkast bevat: {kastBeschrijving}. " +
                             $"Vraag: {question} " +
                             $"Geef een kort, praktisch antwoord in het Nederlands.";

                var requestBody = new
                {
                    model = "llama-3.3-70b-versatile",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 300
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add(
                    "Authorization", $"Bearer {_apiKey}");

                var response = await _httpClient.PostAsync(
                "https://api.groq.com/openai/v1/chat/completions", content);
                var responseJson = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(responseJson);
                return doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();
            }
        }

    }
}