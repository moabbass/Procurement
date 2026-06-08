using Google.Cloud.Firestore;

namespace Procurement.Components.Model
{
    [FirestoreData]
    public class Offer
    {
        public string Id { get; set; }
        public string bidID { get; set; }
        public string supplierEmail { get; set; }
        public string Organization { get; set; }

        // Financial Data
        public double TotalQuoteAmount { get; set; }
        public string Currency { get; set; } = "GBP";

        // Logistics & Timeline
        public int LeadTimeDays { get; set; } // How many days until delivery
        public DateTime OfferExpiryDate { get; set; } // How long this price is valid

        // Technical & Compliance
        public string TechnicalSpecifications { get; set; }
        public bool MeetsAllRequirements { get; set; }
        public string PaymentTerms { get; set; } // e.g., "Net 30"

        // Documentation
        public string fileName { get; set; }
        public string pdfURL { get; set; }

        // Metadata
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending Review"; // Pending, Shortlisted, Rejected, Accepted

        public Offer() { }

    }
}
