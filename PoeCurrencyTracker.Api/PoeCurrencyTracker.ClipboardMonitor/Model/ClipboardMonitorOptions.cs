namespace PoeCurrencyTracker.ClipboardMonitor.Model
{
    internal class ClipboardMonitorOptions
    {
        public const string SectionName = "ClipboardMonitor";

        public string ItemListFileName { get; set; } = "itemlist.json";
        public int ClipboardCheckIntervalMs { get; set; } = 250;
    }
}
