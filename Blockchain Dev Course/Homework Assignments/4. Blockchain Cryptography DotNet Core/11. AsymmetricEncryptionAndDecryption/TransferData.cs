namespace AsymmetricEncryptionAndDecryption
{
    internal class TransferData
    {
        public string RsaEncryptedSecret { get; set; }

        public string AesEncryptedMessage { get; set; }

        public string AesIV { get; set; }

        public string HmacKey { get; set; }

        public string MessageHmac { get; set; }
    }
}
