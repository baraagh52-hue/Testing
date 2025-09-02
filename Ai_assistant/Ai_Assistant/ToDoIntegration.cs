using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Ai_Assistant
{
    public class ToDoIntegration
    {
        private readonly string _todoFilePath = "todo.txt";

        public Task<List<string>> GetToDoItems()
        {
            if (!File.Exists(_todoFilePath))
            {
                return Task.FromResult(new List<string>());
            }

            var lines = File.ReadAllLines(_todoFilePath);
            return Task.FromResult(new List<string>(lines));
        }

        public async Task AddToDoItem(string item)
        {
            if (string.IsNullOrWhiteSpace(item)) { 
                return; 
            }
            var items = await GetToDoItems();
            items.Add(item);
            await File.WriteAllLinesAsync(_todoFilePath, items);
        }
    }
}
