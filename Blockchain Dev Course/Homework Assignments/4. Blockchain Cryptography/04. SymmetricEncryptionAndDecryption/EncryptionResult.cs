namespace SymmetricEncryptionAndDecryption
{
    internal class EncryptionResult
    {
        public SCryptParams Scrypt { get; set; }

        public string Twofish { get; set; }

        public string IV { get; set; }

        public string Mac { get; set; }
    }
}
