namespace Kushl_3m3bdo.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public byte[]? Logo { get; set; }

		public virtual IEnumerable<Product>? Products { get; set; }
	}
}
