namespace Procurement.Components.Model
{
    public class BidTracker : IBidTracker
    {
        public string bidiD;
        public BidTracker() { }
        public virtual string getCurrentBidId()
        {
            return bidiD;
        }

        public void setCurrentBidId(string id)
        {
            bidiD = id;
        }
        public void removeCurrentBid()
        {
            bidiD = string.Empty;
        }

    }
}
