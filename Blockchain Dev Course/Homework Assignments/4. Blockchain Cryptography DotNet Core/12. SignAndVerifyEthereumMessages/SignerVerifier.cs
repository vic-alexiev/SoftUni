namespace SignAndVerifyEthereumMessages
{
    internal class SignerVerifier
    {
        private static void Main(string[] args)
        {
            Signer.Sign();
            Verifier.Verify();
        }
    }
}
