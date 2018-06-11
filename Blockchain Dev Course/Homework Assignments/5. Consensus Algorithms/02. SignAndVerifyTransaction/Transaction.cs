namespace SignAndVerifyTransaction
{
    internal class Transaction
    {
        public string Sender { get; set; }

        public string Recipient { get; set; }

        public string SenderPublicKey { get; set; }

        public int Value { get; set; }

        public int Fee { get; set; }

        public string DateCreatedIso8601 { get; set; }

        public Signature Signature { get; set; }
    }
}
