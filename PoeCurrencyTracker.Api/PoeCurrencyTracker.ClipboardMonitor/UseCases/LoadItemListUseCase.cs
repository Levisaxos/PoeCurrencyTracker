using Microsoft.Extensions.Options;
using PoeCurrencyTracker.ClipboardMonitor.Interface;
using PoeCurrencyTracker.ClipboardMonitor.Model;
using System.Text.Json;

namespace PoeCurrencyTracker.ClipboardMonitor.UseCases
{
    internal class LoadItemListUseCase : ILoadItemListUseCase
    {
        private readonly string _fileName;

        public LoadItemListUseCase(IOptions<ClipboardMonitorOptions> options)
        {
            _fileName = options.Value.ItemListFileName;
        }

        public Dictionary<string, int> Handle()
        {
            // Implementation using _fileName
            if (!File.Exists(_fileName))
                return new Dictionary<string, int>();

            var json = File.ReadAllText(_fileName);
            return JsonSerializer.Deserialize<Dictionary<string, int>>(json) ?? new Dictionary<string, int>();
        }
    }
}
