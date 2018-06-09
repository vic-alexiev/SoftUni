using Common;
using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace SCryptDeriveKeyByPassword
{
    internal class KeyGenerator
    {
        static void Main(string[] args)
        {
            //byte[] salt = EncryptionUtils.GetRandomBytes(256 / 8);
            byte[] salt =
                "7b07a2977a473e84fc30d463a2333bcfea6cb3400b16bec4e17fe981c925ba4f".HexToByteArray();

            byte[] scryptKey = EncryptionUtils.GenerateSCryptKey(
                Utils.GetBytes("p@$$w0rd~3"),
                salt,
                16384,
                16,
                1,
                256);

            Console.WriteLine("salt:{0}{1}{0}scrypt key:{0}{2}",
                Environment.NewLine,
                Utils.ToString(salt),
                Utils.ToString(scryptKey));
        }
    }
}
