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
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Signers;

namespace Node.Api.Helpers
{
    public class CryptographyHelpers : ICryptographyHelpers
    {
        private static readonly X9ECParameters curve = SecNamedCurves.GetByName("secp256k1");

        private static readonly ECDomainParameters domainParameters = new ECDomainParameters(curve.Curve, curve.G, curve.N, curve.H);

        public string CalcSHA256(string text)
        {
            byte[] bytes = CalcSHA256BytesArray(text);

            StringBuilder hash = new StringBuilder();

            foreach (byte b in bytes)
            {
                hash.AppendFormat("{0:X2}", b);
            }

            return hash.ToString();
        }

        public byte[] CalcSHA256BytesArray(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            Sha256Digest digest = new Sha256Digest();

            digest.BlockUpdate(bytes, 0, bytes.Length);

            byte[] result = new byte[digest.GetDigestSize()];

            digest.DoFinal(result, 0);

            return result;
        }

        public string CalcRipeMD160(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);

            RipeMD160Digest digest = new RipeMD160Digest();

            digest.BlockUpdate(bytes, 0, bytes.Length);

            byte[] result = new byte[digest.GetDigestSize()];

            digest.DoFinal(result, 0);

            return this.ConvertByteArrayToHexStringV1(result);
        }

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
            ECPoint publicKey = curve.G.Multiply(privateKey).Normalize();

            return publicKey;
        }

        public string GetPublicKeyCompressed(string privateKeyString)
        {
            BigInteger privateKey = new BigInteger(privateKeyString, 16);
            ECPoint pubKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(pubKey);
            return pubKeyCompressed;
        }

        public string GetAddressFromPrivateKey(string privateKeyHex)
        {
            BigInteger privateKey = new BigInteger(privateKeyHex, 16);

            ECPoint publicKey = GetPublicKeyFromPrivateKey(privateKey);

            string pubKeyCompressed = EncodeECPointHexCompressed(publicKey);

            string address = this.CalcRipeMD160(pubKeyCompressed);

            return address;
        }

        public ECPublicKeyParameters GetPublicKeyParametersFromPrivateKey(string privateKey)
        {
            return this.GetPublicKeyParametersFromPrivateKey(this.ConvertStringToByteArray(privateKey));
        }

        public ECPublicKeyParameters GetPublicKeyParametersFromPrivateKey(byte[] privateKey)
        {
            BigInteger d = new BigInteger(privateKey);
            var q = domainParameters.G.Multiply(d);
            var publicKeyParameters = new ECPublicKeyParameters(q, domainParameters);

            return publicKeyParameters;
        }

        public string EncodeECPointHexCompressed(ECPoint point)
        {
            BigInteger x = point.XCoord.ToBigInteger();

            BigInteger y = point.YCoord.ToBigInteger();

            return x.ToString(16) + Convert.ToInt32(y.TestBit(0));
        }

        public ECPoint DecodeECPointPublicKey(string input)
        {
            BigInteger bigInt = new BigInteger(input, 16);
            byte[] compressedKey = bigInt.ToByteArray();

            var point = curve.Curve.DecodePoint(compressedKey);
            return point;
        }

        public bool VerifySignatureUsingSecp256k1(string publicKey, string[] signature, string message)
        {
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            // byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] messageBytes = CalcSHA256BytesArray(message);

            X9ECParameters parameters = SecNamedCurves.GetByName("secp256k1");
            var ecParameters = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
            ECPoint q = ecParameters.Curve.DecodePoint(publicKeyBytes);
            var publicKeyParameters = new ECPublicKeyParameters(q, ecParameters);

            var signer = new ECDsaSigner();
            signer.Init(false, publicKeyParameters);

            BigInteger[] signatureBigInteger = this.ConvertHexSignatureToBigInteger(signature);

            return signer.VerifySignature(messageBytes, signatureBigInteger[0].Abs(), signatureBigInteger[1].Abs());
        }

        private BigInteger[] ConvertHexSignatureToBigInteger(string[] signature)
        {
            var signatureR = new BigInteger(signature[0], 16);
            var signatureS = new BigInteger(signature[1], 16);

            var signatureBigIntegers = new BigInteger[2];

            signatureBigIntegers[0] = signatureR;
            signatureBigIntegers[1] = signatureS;

            return signatureBigIntegers;
        }

        private string ConvertByteArrayToHexStringV1(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        private string ConvertByteArrayToHexStringV2(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);

            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        private byte[] ConvertStringToByteArray(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return bytes;
        }

        private byte[] ConvertHexStringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];

            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
                
            return bytes;
        }
    }
}
