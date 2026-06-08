using GenerativeAI.Types;
using Google.Cloud.Firestore;

namespace Procurement.Components.Model
{
    public class BidController
    {
        private readonly FirestoreDb _db = FirestoreDb.Create("procurement-ed5cc");
        public BidController() { }
        public BidController(FirestoreDb db)
        {
            _db = db;
        }


        public virtual async Task<Bid> GetBidById(string iD)
        {
            Query query = _db.Collection("Bids").WhereEqualTo("Id", iD).Limit(1);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            var doc = snapshot.Documents.FirstOrDefault();

            if (doc == null)
            {
                return null;
            }
            return new Bid
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Description = doc.TryGetValue("Description", out string desc) ? desc : string.Empty,
                Category = doc.TryGetValue("Category", out string Category) ? Category : string.Empty,                
                open = doc.TryGetValue("Open", out bool isOpen) ? isOpen : false,
                awarded = doc.TryGetValue("awarded", out bool awarded) ? awarded : false,
                Public = doc.TryGetValue("Public", out bool isPublic) ? isPublic : false,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                WinnerOfferId = doc.TryGetValue("WinnerOfferId", out string winnerOfferId) ? winnerOfferId : string.Empty,
                Quantity = doc.TryGetValue("Quantity", out int qty) ? qty : 0,
                EstimatedBudget = doc.TryGetValue("EstimatedBudget", out double budget) ? budget : 0,
                decidedPrice = doc.TryGetValue("EstimatedBudget", out double decidedPrice) ? decidedPrice : 0,
                TechnicalRequirements = doc.TryGetValue("TechnicalRequirements", out string techReq) ? techReq : string.Empty,
                DeliveryLocation = doc.TryGetValue("DeliveryLocation", out string location) ? location : string.Empty,
                dueDate = doc.TryGetValue("dueDate", out DateTime dDate) ? dDate : DateTime.MinValue,
                AwardedAt = doc.TryGetValue("AwardedAt", out DateTime aDate) ? aDate : DateTime.MinValue
            };
        }

        public virtual async Task<List<Bid>> getPublicBids()
        {
            Query query = _db.Collection("Bids").WhereEqualTo("Public", true);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            
            return snapshot.Documents.Select(doc => new Bid
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Description = doc.TryGetValue("Description", out string desc) ? desc : string.Empty,
                Category = doc.TryGetValue("Category", out string Category) ? Category : string.Empty,
                open = doc.TryGetValue("Open", out bool isOpen) ? isOpen : false,
                awarded = doc.TryGetValue("awarded", out bool awarded) ? awarded : false,
                Public = doc.TryGetValue("Public", out bool isPublic) ? isPublic : false,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                WinnerOfferId = doc.TryGetValue("WinnerOfferId", out string winnerOfferId) ? winnerOfferId : string.Empty,
                Quantity = doc.TryGetValue("Quantity", out int qty) ? qty : 0,
                EstimatedBudget = doc.TryGetValue("EstimatedBudget", out double budget) ? budget : 0,
                decidedPrice = doc.TryGetValue("EstimatedBudget", out double decidedPrice) ? decidedPrice : 0,
                TechnicalRequirements = doc.TryGetValue("TechnicalRequirements", out string techReq) ? techReq : string.Empty,
                DeliveryLocation = doc.TryGetValue("DeliveryLocation", out string location) ? location : string.Empty,
                dueDate = doc.TryGetValue("dueDate", out DateTime dDate) ? dDate : DateTime.MinValue,
                AwardedAt = doc.TryGetValue("AwardedAt", out DateTime aDate) ? aDate : DateTime.MinValue
            }).ToList();
        }

        public virtual async Task<List<Bid>> getawardedBids()
        {
            Query query = _db.Collection("Bids").WhereEqualTo("awarded", true);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();


            return snapshot.Documents.Select(doc => new Bid
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Description = doc.TryGetValue("Description", out string desc) ? desc : string.Empty,
                Category = doc.TryGetValue("Category", out string Category) ? Category : string.Empty,
                open = doc.TryGetValue("Open", out bool isOpen) ? isOpen : false,
                awarded = doc.TryGetValue("awarded", out bool awarded) ? awarded : false,
                Public = doc.TryGetValue("Public", out bool isPublic) ? isPublic : false,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                WinnerOfferId = doc.TryGetValue("WinnerOfferId", out string winnerOfferId) ? winnerOfferId : string.Empty,
                Quantity = doc.TryGetValue("Quantity", out int qty) ? qty : 0,
                EstimatedBudget = doc.TryGetValue("EstimatedBudget", out double budget) ? budget : 0,
                decidedPrice = doc.TryGetValue("EstimatedBudget", out double decidedPrice) ? decidedPrice : 0,
                TechnicalRequirements = doc.TryGetValue("TechnicalRequirements", out string techReq) ? techReq : string.Empty,
                DeliveryLocation = doc.TryGetValue("DeliveryLocation", out string location) ? location : string.Empty,
                dueDate = doc.TryGetValue("dueDate", out DateTime dDate) ? dDate : DateTime.MinValue,
                AwardedAt = doc.TryGetValue("AwardedAt", out DateTime aDate) ? aDate : DateTime.MinValue
            }).ToList();
        }

        public virtual async Task<List<Bid>> getAllBids()
        {
            Query query = _db.Collection("Bids");
            QuerySnapshot snapshot = await query.GetSnapshotAsync();


            return snapshot.Documents.Select(doc => new Bid
            {
                Id = doc.TryGetValue("Id", out string ID) ? ID : string.Empty,
                Description = doc.TryGetValue("Description", out string desc) ? desc : string.Empty,
                Category = doc.TryGetValue("Category", out string Category) ? Category : string.Empty,
                open = doc.TryGetValue("Open", out bool isOpen) ? isOpen : false,
                awarded = doc.TryGetValue("awarded", out bool awarded) ? awarded : false,
                Public = doc.TryGetValue("Public", out bool isPublic) ? isPublic : false,
                fileName = doc.TryGetValue("fileName", out string fName) ? fName : string.Empty,
                WinnerOfferId = doc.TryGetValue("WinnerOfferId", out string winnerOfferId) ? winnerOfferId : string.Empty,
                Quantity = doc.TryGetValue("Quantity", out int qty) ? qty : 0,
                EstimatedBudget = doc.TryGetValue("EstimatedBudget", out double budget) ? budget : 0,
                decidedPrice = doc.TryGetValue("EstimatedBudget", out double decidedPrice) ? decidedPrice : 0,
                TechnicalRequirements = doc.TryGetValue("TechnicalRequirements", out string techReq) ? techReq : string.Empty,
                DeliveryLocation = doc.TryGetValue("DeliveryLocation", out string location) ? location : string.Empty,
                dueDate = doc.TryGetValue("dueDate", out DateTime dDate) ? dDate : DateTime.MinValue,
                AwardedAt = doc.TryGetValue("AwardedAt", out DateTime aDate) ? aDate : DateTime.MinValue
            }).ToList();
        }


    }
}
