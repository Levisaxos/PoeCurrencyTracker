using PoeCurrencyTracker.ClipboardMonitor.Interface;
using System.Text.RegularExpressions;

namespace PoeCurrencyTracker.ClipboardMonitor.UseCases
{
    internal class GetPoeItemUseCase : IGetPoeItemUseCase
    {
        public (string name, int count) Handle(string clipboardText)
        {
            var textArray = clipboardText.Split(["\r\n", "\n"], StringSplitOptions.None);
            if (textArray.Length < 5)
                return (String.Empty, 0);

            var itemName = textArray[2];
            var stackItem = textArray[4];

            if (!stackItem.Contains("/"))
                return (String.Empty, 0);

            var match = Regex.Match(stackItem, @"(\d+)/");
            if (match.Success && int.TryParse(match.Groups[1].Value, out int stackCount))
                return (itemName, stackCount);

            return (String.Empty, 0);
        }
    }
}
