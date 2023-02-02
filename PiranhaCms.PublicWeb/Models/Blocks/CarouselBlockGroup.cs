using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.PublicWeb.Business.Constants;
using PiranhaCMS.PublicWeb.Models.Blocks.Base;

namespace PiranhaCMS.PublicWeb.Models.Blocks;

[BlockGroupType(
    Name = "Carousel Block Group",
    Display = BlockDisplayMode.Vertical,
    Category = Global.CarouselCategory)]
[BlockItemType(Type = typeof(CarouselItemBlock))]
public class CarouselBlockGroup : BlockGroupBase
{ }
