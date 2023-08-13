using Piranha.Models;

namespace PiranhaCMS.ContentTypes.Blocks.Base;

public interface ICurrentPage
{
    PageBase CurrentPage { get; }
}
