using Piranha.Extend;
using Piranha.Models;
using PiranhaCMS.ContentTypes.Blocks.Base;
using PiranhaCMS.ContentTypes.Constants;

namespace PiranhaCMS.ContentTypes.Blocks;

[BlockGroupType(
    Name = "Carousel Block Group",
    Display = BlockDisplayMode.Vertical,
    Category = Global.CarouselCategory)]
[BlockItemType(Type = typeof(CarouselItemBlock))]
public class CarouselBlockGroup : BlockGroupBase
{ }
