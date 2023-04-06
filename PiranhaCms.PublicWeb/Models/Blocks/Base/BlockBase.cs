using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.Common.Helpers;

namespace PiranhaCMS.PublicWeb.Models.Blocks.Base;

public abstract class BlockBase : Block, ICurrentPage
{
    public PageBase CurrentPage => PageHelpers.GetCurrentPage();
}
