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

namespace HaloSigner.IO
{
    public class BigEndianStream
    {
        public BigEndianStream(Stream stream)
        {
            _reader = new BigEndianReader(stream);
            _writer = new BigEndianWriter(stream);
        }

        public byte ReadByte()
        {
            return _reader.ReadByte();
        }

        public void WriteByte(byte b)
        {
            _writer.WriteByte(b);
        }

        public sbyte ReadSByte()
        {
            return _reader.ReadSByte();
        }

        public void WriteSByte(sbyte b)
        {
            _writer.WriteSByte(b);
        }

        public ushort ReadUInt16()
        {
            return _reader.ReadUInt16();
        }

        public void WriteUInt16(ushort value)
        {
            _writer.WriteUInt16(value);
        }

        public short ReadInt16()
        {
            return _reader.ReadInt16();
        }

        public void WriteInt16(short value)
        {
            _writer.WriteInt16(value);
        }

        public uint ReadUInt32()
        {
            return _reader.ReadUInt32();
        }

        public void WriteUInt32(uint value)
        {
            _writer.WriteUInt32(value);
        }

        public int ReadInt32()
        {
            return _reader.ReadInt32();
        }

        public void WriteInt32(int value)
        {
            _writer.WriteInt32(value);
        }

        public ulong ReadUInt64()
        {
            return _reader.ReadUInt64();
        }

        public string ReadAscii()
        {
            return _reader.ReadAscii();
        }

        public string ReadUTF16()
        {
            return _reader.ReadUTF16();
        }

        public void WriteUTF16(string str)
        {
            _writer.WriteUTF16(str);
        }

        public byte[] ReadBlock(int size)
        {
            return _reader.ReadBlock(size);
        }

        public void ReadBlock(byte[] output, int offset, int size)
        {
            _reader.ReadBlock(output, offset, size);
        }

        public void WriteBlock(byte[] data)
        {
            _writer.WriteBlock(data);
        }

        public void Close()
        {
            _reader.Close();
            _writer.Close();
        }

        public long Position
        {
            get { return _reader.Position; }
            set { _reader.SeekTo(value); }
        }

        public long Length
        {
            get { return _reader.Length; }
        }

        public bool EOF
        {
            get { return _reader.EOF; }
        }

        private BigEndianReader _reader;
        private BigEndianWriter _writer;
    }
}
