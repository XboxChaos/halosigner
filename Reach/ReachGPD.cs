/*
 * Copyright (C) 2011 Aaron Dierking
 * This file is part of halosigner.
 *
 * halosigner is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * halosigner is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with halosigner.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using HaloSigner.Containers;
using HaloSigner.IO;

namespace HaloSigner.Reach
{
    public class ReachGPD : GPD
    {
        public ReachGPD(string path)
            : base(path)
        {
            try
            {
                ReadSettings();
            }
            catch
            {
                Close();
                throw;
            }
        }

        public ReachGPD(Stream stream)
            : base(stream)
        {
            ReadSettings();
        }

        /// <summary>
        /// The GPD's hash.
        /// </summary>
        public byte[] Hash
        {
            get { return _hash; }
        }

        /// <summary>
        /// Updates and rehashes the GPD.
        /// </summary>
        public void Update()
        {
            // Not much to see here...
            WriteHash();
        }

        private void ReadSettings()
        {
            if (TitleName != "Halo: Reach")
                throw new ArgumentException("Not a valid Reach GPD");

            // Title-specific data 1
            _titleSpecific1 = FindSetting(TitleSpecific1ID) as GPDBinarySetting;
            if (_titleSpecific1 == null || _titleSpecific1.Size != TitleSpecific1Size)
                throw new ArgumentException("Not a valid Reach GPD - missing or invalid TitleSpecific1");

            // Title-specific data 2
            _titleSpecific2 = FindSetting(TitleSpecific2ID) as GPDBinarySetting;
            if (_titleSpecific2 == null || _titleSpecific2.Size != TitleSpecific2Size)
                throw new ArgumentException("Not a valid Reach GPD - missing or invalid TitleSpecific2");

            // Title-specific data 3
            _titleSpecific3 = FindSetting(TitleSpecific3ID) as GPDBinarySetting;
            if (_titleSpecific3 == null || _titleSpecific3.Size != TitleSpecific3Size)
                throw new ArgumentException("Not a valid Reach GPD - missing or invalid TitleSpecific3");

            ReadHash();
        }

        private void ReadHash()
        {
            BigEndianStream stream = _titleSpecific3.GetStream();
            stream.Position += HashOffset;
            _hash = new byte[HashSize];
            stream.ReadBlock(_hash, 0, HashSize);
        }

        private void WriteHash()
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            const int LargestSettingSize = 0x3E8;
            byte[] data = new byte[LargestSettingSize];

            // Hash TitleSpecific1
            BigEndianStream stream = _titleSpecific1.GetStream();
            stream.ReadBlock(data, 0, TitleSpecific1Size);
            sha1.TransformBlock(data, 0, TitleSpecific1Size, data, 0);

            // Hash TitleSpecific2
            stream = _titleSpecific2.GetStream();
            stream.ReadBlock(data, 0, TitleSpecific2Size);
            sha1.TransformBlock(data, 0, TitleSpecific2Size, data, 0);

            // Read in TitleSpecific3
            stream = _titleSpecific3.GetStream();
            stream.ReadBlock(data, 0, TitleSpecific3Size);

            // Fill the hash area with 0x99
            for (int i = HashOffset; i < HashOffset + HashSize; i++)
                data[i] = 0x99;

            // Transform TitleSpecific3 and get the hash
            sha1.TransformFinalBlock(data, 0, TitleSpecific3Size);
            _hash = sha1.Hash;

            // Write it back
            stream.Position -= TitleSpecific3Size - HashOffset;
            stream.WriteBlock(_hash);
        }

        private GPDBinarySetting _titleSpecific1;
        private GPDBinarySetting _titleSpecific2;
        private GPDBinarySetting _titleSpecific3;

        private byte[] _hash;

        const int HashOffset = 0x2E8;
        const int HashSize = 0x14;
        const uint TitleSpecific1ID = 0x63E83FFF;
        const uint TitleSpecific2ID = 0x63E83FFE;
        const uint TitleSpecific3ID = 0x63E83FFD;
        const int TitleSpecific1Size = 0x3E8;
        const int TitleSpecific2Size = 0x3E8;
        const int TitleSpecific3Size = 0x300;
    }
}
