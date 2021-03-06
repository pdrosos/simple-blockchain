﻿using System;
using System.Text;
using System.Linq;
using System.Security.Cryptography;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Signers;
using ECPoint = Org.BouncyCastle.Math.EC.ECPoint;

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

        public byte[] CalcSHA256BytesArrayDotNet(string text)
        {
            return SHA256Managed.Create().ComputeHash(Encoding.UTF8.GetBytes(text));
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

        public bool VerifySignatureUsingSecp256k1(byte[] publicKey, BigInteger[] signature, byte[] message)
        {
            X9ECParameters parameters = SecNamedCurves.GetByName("secp256k1");
            var ecParameters = new ECDomainParameters(parameters.Curve, parameters.G, parameters.N, parameters.H);
            ECPoint q = ecParameters.Curve.DecodePoint(publicKey);
            var publicKeyParameters = new ECPublicKeyParameters(q, ecParameters);

            var signer = new ECDsaSigner();
            signer.Init(false, publicKeyParameters);

            return signer.VerifySignature(message, signature[0].Abs(), signature[1].Abs());
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

        private string ConvertByteArrayToHexStringV1(byte[] bytes)
        {
            return string.Concat(bytes.Select(b => b.ToString("x2")));
        }

        public byte[] ConvertStringToByteArray(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            return bytes;
        }

        public byte[] ConvertHexStringToByteArray(string hex)
        {
            byte[] bytes = Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();

            return bytes;
        }

        public byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();

            return hashstring.ComputeHash(array);
        }

        public string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            string hexAlphabet = "0123456789ABCDEF";

            foreach (byte b in bytes)
            {
                result.Append(hexAlphabet[(int)(b >> 4)]);
                result.Append(hexAlphabet[(int)(b & 0x0F)]);
            }

            return result.ToString().ToLower();
        }
    }
}
