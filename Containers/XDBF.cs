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
using HaloSigner.IO;

namespace HaloSigner.Containers
{
    /// <summary>
    /// Provides (very) low-level methods for reading and writing XDBF files.
    /// </summary>
    public class XDBF
    {
        /// <summary>
        /// Loads an XDBF container from a file.
        /// </summary>
        /// <param name="path">The path to the file to load</param>
        public XDBF(string path)
        {
            _stream = new BigEndianStream(File.Open(path, FileMode.Open, FileAccess.ReadWrite));
            try
            {
                ProcessFile();
            }
            catch
            {
                _stream.Close();
                throw;
            }
        }

        /// <summary>
        /// Reads an XDBF container from a Stream.
        /// </summary>
        /// <param name="stream">The Stream to read from</param>
        public XDBF(Stream stream)
        {
            _stream = new BigEndianStream(stream);
            ProcessFile();
        }

        /// <summary>
        /// The callback type for EnumEntries.
        /// </summary>
        /// <param name="container">The XDBF this callback was called from</param>
        /// <param name="entry">The entry that was read</param>
        /// <param name="stream">A stream which can be used to read the entry (it will not point to it by default!)</param>
        public delegate void EnumEntriesCallback(XDBF container, XDBFEntry entry, BigEndianStream stream);

        /// <summary>
        /// Scans through the entry table and calls a callback function for each entry.
        /// </summary>
        /// <param name="callback">The EnumEntriesCallback to call</param>
        public void EnumEntries(EnumEntriesCallback callback)
        {
            foreach (XDBFEntry entry in _entries)
            {
                callback(this, entry, _stream);
            }
        }

        /// <summary>
        /// Closes any open streams for this XDBF container.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        private void ProcessFile()
        {
            ReadHeader();
            ReadEntries();
        }

        private void ReadHeader()
        {
            const uint Magic = 0x58444246;
            const uint ExpectedVersion = 0x10000;

            if (_stream.ReadUInt32() != Magic)
                throw new ArgumentException("Invalid XDBF file - bad header");
            if (_stream.ReadUInt32() != ExpectedVersion)
                throw new ArgumentException("Invalid XDBF file - unsupported version");

            _entryTableLength = _stream.ReadUInt32();
            _entryCount = _stream.ReadUInt32();
            _freeSpaceTableLength = _stream.ReadUInt32();
            _freeSpaceTableCount = _stream.ReadUInt32();
        }

        private void ReadEntries()
        {
            uint entryDataStart = HeaderSize + _entryTableLength * EntrySize + _freeSpaceTableLength * FreeSpaceEntrySize;

            _entries = new List<XDBFEntry>();
            _stream.Position = EntryTableOffset;
            for (uint i = 0; i < _entryCount; i++)
            {
                XDBFEntry entry = new XDBFEntry(_stream, entryDataStart);
                _entries.Add(entry);
            }
        }

        private BigEndianStream _stream;
        private uint _entryTableLength;
        private uint _entryCount;
        private uint _freeSpaceTableLength;
        private uint _freeSpaceTableCount;
        private List<XDBFEntry> _entries;

        const uint HeaderSize = 0x18;
        const uint EntryTableOffset = HeaderSize;
        const uint EntrySize = 0x12;
        const uint FreeSpaceEntrySize = 0x8;
    }
}
