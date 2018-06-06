namespace SymmetricEncryptionAndDecryption
{
    internal class SCryptParams
    {
        public int Dklen { get; set; }

        public string Salt { get; set; }

        public int N { get; set; }

        public int R { get; set; }

        public int P { get; set; }
    }
}
