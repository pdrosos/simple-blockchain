using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;

namespace Node.Api.Helpers
{
    public interface ICryptographyHelpers
    {
        string CalcSHA256(string text);

        byte[] CalcSHA256BytesArray(string text);

        string CalcRipeMD160(string text);

        AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256);

        ECPoint GetPublicKeyFromPrivateKey(BigInteger privateKey);

        string GetAddressFromPrivateKey(string privateKeyHex);

        ECPublicKeyParameters GetPublicKeyParametersFromPrivateKey(string privateKey);

        ECPublicKeyParameters GetPublicKeyParametersFromPrivateKey(byte[] privateKey);

        string EncodeECPointHexCompressed(ECPoint point);

        ECPoint DecodeECPointPublicKey(string input);

        bool VerifySignatureUsingSecp256k1(string publicKey, string[] signature, string message);
    }
}
