using System;
using System.Text;
using System.Linq;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Digests;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Signers;

namespace Node.Api.Helpers
{
    public class Crypto
    {
        static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");

        public AsymmetricCipherKeyPair GenerateRandomKeys(int keySize = 256)
        {
            ECKeyPairGenerator gen = new ECKeyPairGenerator();

            SecureRandom secureRandom = new SecureRandom();

            KeyGenerationParameters keyGenParam = new KeyGenerationParameters(secureRandom, keySize);

            gen.Init(keyGenParam);

            return gen.GenerateKeyPair();
        }

        public ECPoint GetPublicKeyFromPrivateKey(BigInteger privateKey)
        {
            ECPoint pubKey = curve.G.Multiply(privateKey).Normalize();

            return pubKey;
        }

        public string GetAddressFromPrivateKey(string privateKeyHex)
        {
            BigInteger privateKey = new BigInteger(privateKeyHex, 16);

            ECPoint publicKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(publicKey);

            string address = CalcRipeMD160(pubKeyCompressed);

            return address;
        }

        public string BytesToHex(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public string EncodeECPointHexCompressed(ECPoint point)
        {
            BigInteger x = point.XCoord.ToBigInteger();

            BigInteger y = point.YCoord.ToBigInteger();

            return x.ToString(16) + Convert.ToInt32(y.TestBit(0));
        }

        public string CalcRipeMD160(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            RipeMD160Digest digest = new RipeMD160Digest();

            digest.BlockUpdate(bytes, 0, bytes.Length);

            byte[] result = new byte[digest.GetDigestSize()];

            digest.DoFinal(result, 0);

            return this.BytesToHex(result);
        }

        private byte[] CalcSHA256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            Sha256Digest digest = new Sha256Digest();

            digest.BlockUpdate(bytes, 0, bytes.Length);

            byte[] result = new byte[digest.GetDigestSize()];

            digest.DoFinal(result, 0);

            return result;
        }

        public bool VerifySignatureUsingSecp256k1(string publicKey, string[] signature, string message)
        {
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            X9ECParameters parameters = SecNamedCurves.GetByName("secp256k1");
            var ecParameters = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
            var publicKeyParameters = new ECPublicKeyParameters(ecParameters.Curve.DecodePoint(publicKeyBytes), ecParameters);

            var signer = new ECDsaSigner();
            signer.Init(false, publicKeyParameters);

            BigInteger[] signatureBigInteger = this.ConvertHexSignatureToBigInteger(signature);

            return signer.VerifySignature(messageBytes, signatureBigInteger[0].Abs(), signatureBigInteger[1].Abs());
        }

        public BigInteger[] ConvertHexSignatureToBigInteger(string[] signature)
        {
            var signatureR = new BigInteger(signature[0], 16);
            var signatureS = new BigInteger(signature[1], 16);

            var signatureBigIntegers = new BigInteger[2];

            signatureBigIntegers[0] = signatureR;
            signatureBigIntegers[1] = signatureS;

            return signatureBigIntegers;
        }
    }
}
