namespace PoeCurrencyTracker.ClipboardMonitor.Interface
{
    internal interface ISaveItemListUseCase
    {
        bool Handle(Dictionary<string, int> data);        
    }
}