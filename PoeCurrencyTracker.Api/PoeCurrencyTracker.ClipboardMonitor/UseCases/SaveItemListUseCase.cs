using Microsoft.Extensions.Options;
using PoeCurrencyTracker.ClipboardMonitor.Interface;
using PoeCurrencyTracker.ClipboardMonitor.Model;
using System.Text.Json;

namespace PoeCurrencyTracker.ClipboardMonitor.UseCases
{
    internal class SaveItemListUseCase : ISaveItemListUseCase
    {
        private readonly string _fileName;
        public SaveItemListUseCase(IOptions<ClipboardMonitorOptions> options)
        {
            _fileName = options.Value.ItemListFileName;
        }

        public bool Handle(Dictionary<string, int> data)
        {
            try
            {
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_fileName, json);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}