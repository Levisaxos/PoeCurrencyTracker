namespace PoeCurrencyTracker.ClipboardMonitor.Interface
{
    internal interface IGetPoeItemUseCase
    {
        (string name, int count) Handle(string clipboardText);
    }
}
