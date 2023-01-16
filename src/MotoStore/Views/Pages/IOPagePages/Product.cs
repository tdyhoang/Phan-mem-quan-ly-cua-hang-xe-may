namespace MotoStore.Views.Pages.IOPagePages
{
    public class Product
    {
        public string? ProductId { get; set; }
        public string? NameProduct { get; set; }
        public decimal? ValueMoney { get; set; }
        public string? Mau { get; set; }
        public string? Image { get; set; }

        public Product(string productName, decimal? valueMoney, string image) //Lưu ý : ValueMoney phải là DECIMAL
        {
            NameProduct = productName;
            ValueMoney = valueMoney;
            Image = image;
        }
    }
}
