using PoeCurrencyTracker.ClipboardMonitor.Interface;

namespace PoeCurrencyTracker.ClipboardMonitor.UseCases
{
    internal class GetClipboardTextUseCase : IGetClipboardTextUseCase
    {
        private static readonly object _clipboardLock = new object();
        private string lastClipboardContent = "";

        public string Handle()
        {
            lock (_clipboardLock)
            {
                try
                {
                    if (Clipboard.ContainsText())
                    {
                        var text = Clipboard.GetText();
                        if (text != lastClipboardContent && !string.IsNullOrWhiteSpace(text))
                        {
                            lastClipboardContent = text;

                            // Don't clear immediately - can cause hangs
                            // Instead, clear after a delay
                            Task.Run(async () =>
                            {
                                await Task.Delay(100);
                                try { Clipboard.Clear(); } catch { }
                            });

                            return text;
                        }
                    }
                }
                catch
                {
                    // Silent fail - clipboard access can be temperamental
                }
            }
            return null;
        }
    }
}