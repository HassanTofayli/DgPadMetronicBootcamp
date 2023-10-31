using Microsoft.AspNetCore.Mvc;

namespace ShoppingCartAdminMetronic.Data.Components
{
	public class ProfileCardViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke() => View();

	}
}
