namespace Kushk_3m3bdo.Models.ViewModels
{
	public class ShoppingCartsViewModel
	{
		public OrderHeader OrderHeader { get; set; }
		public IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
	}
}
