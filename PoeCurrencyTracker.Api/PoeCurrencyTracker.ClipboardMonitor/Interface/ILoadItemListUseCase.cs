using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCurrencyTracker.ClipboardMonitor.Interface
{
    internal interface ILoadItemListUseCase
    {
        Dictionary<string, int> Handle();
    }
}
