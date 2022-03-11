namespace FindstakeNet
{
    public class StakeTemplate
    {
        public string ID { get; set; }
        public string Address { get; set; }
        public string Transaction { get; set; }
        public string Index { get; set; }
        public string StakesFound { get; set; }

        public StakeTemplate(string id, string address, string transaction, string index, string found)
        {
            this.ID = id;
            this.Address = address;
            this.Transaction = transaction;
            this.Index = index;
            this.StakesFound = found;
        }
    }
}
