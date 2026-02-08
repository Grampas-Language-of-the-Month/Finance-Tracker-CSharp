using System.Text.Json;

namespace finance_tracker.Services
{
    public class CategoryService
    {
        private readonly Dictionary<string, string> _keywordMap = new();

        public CategoryService()
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                string jsonString = File.ReadAllText("keywords.json");
                var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(jsonString);

                if (data != null)
                {
                    foreach (var category in data)
                    {
                        foreach (var keyword in category.Value)
                        {
                            _keywordMap[keyword.ToLower()] = category.Key;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading JSON: {ex.Message}");
            }
        }

        public string GetCategory(string description)
        {
            if (string.IsNullOrEmpty(description)) return "Misc";

            description = description.ToLower();

            foreach (var entry in _keywordMap)
            {
                if (description.Contains(entry.Key))
                {
                    return entry.Value;
                }
            }

            return "Misc";
        }
    }
}