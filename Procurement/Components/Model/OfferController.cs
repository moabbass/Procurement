using Google.Cloud.Firestore;

namespace Procurement.Components.Model
{
    public class OfferController
    {
        private readonly FirestoreDb _db = FirestoreDb.Create("procurement-ed5cc");
        public OfferController() { }
        public OfferController(FirestoreDb db)
        {
            _db = db;
        }

        public async Task<List<Offer>> GetOffers(string email)
        {
            // Query the 'users' collection where 'Category' == selected category
            Query query = _db.Collection("Offers").WhereEqualTo("supplierEmail", email);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => new Offer
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Organization = doc.TryGetValue("Organization", out string org) ? org : string.Empty,
                PaymentTerms = doc.TryGetValue("PaymentTerms", out string PaymentTerms) ? PaymentTerms : string.Empty,
                Status = doc.TryGetValue("Status", out string Status) ? Status : string.Empty,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                bidID = doc.TryGetValue("bidID", out string bidID) ? bidID : string.Empty,
                LeadTimeDays = doc.TryGetValue("LeadTimeDays", out int leadTime) ? leadTime : 0,
                TotalQuoteAmount = doc.TryGetValue("TotalQuoteAmount", out double TotalQuoteAmount) ? TotalQuoteAmount : 0,                
                TechnicalSpecifications = doc.TryGetValue("TechnicalSpecifications", out string techspec) ? techspec : string.Empty,
                supplierEmail = doc.TryGetValue("supplierEmail", out string supplierEmail) ? supplierEmail : string.Empty,
                OfferExpiryDate = doc.TryGetValue("dueDate", out DateTime expiryDate) ? expiryDate : DateTime.MinValue
            }).ToList();            
            
        }

        public async Task<Offer> GetOfferByID(string Id)
        {
            // Query the 'users' collection where 'Category' == selected category
            Query query = _db.Collection("Offers").WhereEqualTo("Id", Id);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            var doc = snapshot.Documents.FirstOrDefault();

            if (doc == null)
            {
                return null;
            }

            return new Offer
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Organization = doc.TryGetValue("Organization", out string org) ? org : string.Empty,
                PaymentTerms = doc.TryGetValue("PaymentTerms", out string PaymentTerms) ? PaymentTerms : string.Empty,
                Status = doc.TryGetValue("Status", out string Status) ? Status : string.Empty,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                bidID = doc.TryGetValue("bidID", out string bidID) ? bidID : string.Empty,
                LeadTimeDays = doc.TryGetValue("LeadTimeDays", out int leadTime) ? leadTime : 0,
                TotalQuoteAmount = doc.TryGetValue("TotalQuoteAmount", out double TotalQuoteAmount) ? TotalQuoteAmount : 0,
                TechnicalSpecifications = doc.TryGetValue("TechnicalSpecifications", out string techspec) ? techspec : string.Empty,
                supplierEmail = doc.TryGetValue("supplierEmail", out string supplierEmail) ? supplierEmail : string.Empty,
                OfferExpiryDate = doc.TryGetValue("dueDate", out DateTime expiryDate) ? expiryDate : DateTime.MinValue
            };

        }
    }
}
