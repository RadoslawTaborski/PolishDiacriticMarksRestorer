using System.Collections.Generic;

namespace NgramFilter.Interfaces
{
    public interface IFilterItem
    {
        bool IsCorrect(List<string> strList);
    }
}
