namespace Procurement.Components.Model
{
    public class Bid
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public bool Public { get; set; }
        public string Category { get; set; }
        public DateTime dueDate {  get; set; }
        public bool Published {  get; set; }
        public bool open {  get; set; }
        public List<string> supliersList { get; set; }
        public string fileName { get; set; }
        public string PdfUrl { get; set; }
        public int Quantity { get; set; } = 1;
        public double EstimatedBudget { get; set; }
        public string TechnicalRequirements { get; set; }
        public string DeliveryLocation { get; set; }
        public string WinnerOfferId { get; set; }        
        public DateTime AwardedAt { get; set; }
        public double decidedPrice { get; set; }
        public bool awarded {  get; set; }

        public Bid()
        {
            supliersList = new List<string>();
        }
    }
}
