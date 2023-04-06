using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PiranhaCMS.PublicWeb.Components
{
    public class Test : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var items = new List<string> { "Test", "Test 1" };
            return View(items);
        }
    }
}
