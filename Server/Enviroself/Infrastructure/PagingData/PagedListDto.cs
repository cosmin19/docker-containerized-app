using System.Collections.Generic;

namespace Enviroself.Infrastructure.PagingData
{
    public class PagedListDto<T>
    {
        public IList<T> List;
        public PagingHeader PagingHeader { get; set; }
    }
}
