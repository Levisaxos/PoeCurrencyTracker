using PoeCurrencyTracker.ClipboardMonitor.Interface;

namespace PoeCurrencyTracker.ClipboardMonitor.UseCases
{
    internal class ClipboardMonitorUseCase : IClipboardMonitorUseCase
    {
        private static Dictionary<string, int> Items = new Dictionary<string, int>();
        private readonly IGetClipboardTextUseCase _getClipboardTextUseCase;
        private readonly IGetPoeItemUseCase _getPoeItemUseCase;
        private readonly ILoadItemListUseCase _loadItemListUseCase;
        private readonly ISaveItemListUseCase _saveItemListUseCase;
        public ClipboardMonitorUseCase(ILoadItemListUseCase loadItemListUseCase, ISaveItemListUseCase saveItemListUseCase, IGetClipboardTextUseCase getClipboardTextUseCase, IGetPoeItemUseCase getPoeItemUseCase)
        {
            _loadItemListUseCase = loadItemListUseCase;
            _saveItemListUseCase = saveItemListUseCase;
            _getPoeItemUseCase = getPoeItemUseCase; ;
            _getClipboardTextUseCase = getClipboardTextUseCase;

        }
        public void Handle()
        {
            Items = _loadItemListUseCase.Handle();

            Console.WriteLine("Item tracker started. Press 'q' to quit.");
            while (true)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        if (key.KeyChar == 'q' || key.KeyChar == 'Q')
                            break;
                    }

                    Thread.Sleep(250);
                    var text = _getClipboardTextUseCase.Handle();
                    if (string.IsNullOrEmpty(text))
                        continue;

                    var item = _getPoeItemUseCase.Handle(text);
                    if (string.IsNullOrEmpty(item.name))
                        continue;

                    if (!Items.ContainsKey(item.name))
                    {
                        Items.Add(item.name, item.count);
                        _saveItemListUseCase.Handle(Items);
                        Console.WriteLine($"Added {item.name}: {item.count}");
                    }
                    else if (Items[item.name] != item.count)
                    {
                        Items[item.name] = item.count;
                        _saveItemListUseCase.Handle(Items);
                        Console.WriteLine($"Updated {item.name}: {item.count}");
                    }
                    else
                        Console.WriteLine($"No update: {item.name}");
                }
                catch (Exception ex)
                {
                    // Only log significant errors, ignore clipboard access issues
                    if (!ex.Message.Contains("clipboard"))
                        Console.WriteLine($"Error: {ex.Message}");
                    Thread.Sleep(500); // Longer delay on error
                }
            }
        }
    }
}
