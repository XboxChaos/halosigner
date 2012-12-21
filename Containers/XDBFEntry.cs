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
using HaloSigner.IO;

namespace HaloSigner.Containers
{
    /// <summary>
    /// Represents an entry in an XDBF container.
    /// </summary>
    public class XDBFEntry
    {
        public XDBFEntry(BigEndianStream stream, uint entryDataStartOffset)
        {
            ReadFromStream(stream);
            _fileOffset = entryDataStartOffset + _offset;
        }

        private void ReadFromStream(BigEndianStream stream)
        {
            _namespace = stream.ReadUInt16();
            _id = stream.ReadUInt64();
            _offset = stream.ReadUInt32();
            _size = stream.ReadInt32();
        }

        /// <summary>
        /// The entry's namespace. This is specific to the file type.
        /// </summary>
        public ushort Namespace
        {
            get { return _namespace; }
        }

        /// <summary>
        /// The entry's ID.
        /// </summary>
        public ulong ID
        {
            get { return _id; }
        }

        /// <summary>
        /// The entry's file offset.
        /// </summary>
        public uint FileOffset
        {
            get { return _fileOffset; }
        }

        /// <summary>
        /// The entry's size in bytes.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        private ushort _namespace;
        private ulong _id;
        private uint _fileOffset;
        private int _size;
        private uint _offset;
    }
}
