namespace Procurement.Components.Model
{
    public interface IBidTracker
    {
        /// <summary>
        /// Retrieves the ID of the currently active/selected bid.
        /// </summary>
        string getCurrentBidId();

        /// <summary>
        /// Sets the ID of the currently active/selected bid.
        /// </summary>
        void setCurrentBidId(string id);

        /// <summary>
        /// Clears the currently active bid ID.
        /// </summary>
        void removeCurrentBid();
    }
}