using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace HaloSigner.Blam3
{
    /// <summary>
    /// Computes salted SHA-1 digests using Reach's key.
    /// </summary>
    class SaltedSHA1 : SHA1Managed
    {
        private static byte[] _salt = new byte[]{
            0xED, 0xD4, 0x30, 0x09, 0x66, 0x6D, 0x5C, 0x4A, 0x5C, 0x36, 0x57,
            0xFA, 0xB4, 0x0E, 0x02, 0x2F, 0x53, 0x5A, 0xC6, 0xC9, 0xEE, 0x47,
            0x1F, 0x01, 0xF1, 0xA4, 0x47, 0x56, 0xB7, 0x71, 0x4F, 0x1C, 0x36, 
            0xEC};

        /// <summary>
        /// Constructs and initializes a new SaveSHA1 object.
        /// </summary>
        /// <seealso cref="Initialize"/>
        public SaltedSHA1()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes the hash by transforming the salt.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            base.TransformBlock(_salt, 0, 34, _salt, 0);
        }
    }
}
