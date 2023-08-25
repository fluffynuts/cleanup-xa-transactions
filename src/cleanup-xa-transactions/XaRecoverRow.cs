namespace cleanup_xa_transactions
{
    public class XaRecoverRow
    {
        public int FormatId { get; set; }
        public int GTRID_Length { get; }
        public int BQUAL_Length { get; }
        public string Data { get; }
    }
}