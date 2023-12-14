using Microsoft.AspNetCore.Mvc;

namespace PiranhaCMS.PublicWeb.Components;

public class Test : ViewComponent
{
	public async Task<IViewComponentResult> InvokeAsync()
	{
		var items = new List<string> { "Test", "Test 1" };
		return View(items);
	}
}
