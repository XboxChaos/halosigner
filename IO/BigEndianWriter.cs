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
    /// <summary>
    /// Writes big-endian data to a stream.
    /// Designed to support the various types that Bungie uses.
    /// </summary>
    public class BigEndianWriter
    {
        /// <summary>
        /// Constructs a new BigEndianWriter.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        public BigEndianWriter(Stream stream)
        {
            _stream = stream;
        }

        /// <summary>
        /// Closes the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Writes a byte to the underlying stream.
        /// </summary>
        /// <param name="value">The byte to write.</param>
        public void WriteByte(byte value)
        {
            _stream.WriteByte(value);
        }

        /// <summary>
        /// Writes a signed byte to the underlying stream.
        /// </summary>
        /// <param name="value">The signed byte to write.</param>
        public void WriteSByte(sbyte value)
        {
            _stream.WriteByte((byte)value);
        }

        /// <summary>
        /// Writes an unsigned 16-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt16(ushort value)
        {
            _stream.WriteByte((byte)(value >> 8));
            _stream.WriteByte((byte)(value & 0xFF));
        }

        /// <summary>
        /// Writes a signed 16-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt16(short value)
        {
            WriteUInt16((ushort)value);
        }

        /// <summary>
        /// Writes an unsigned 32-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt32(uint value)
        {
            _stream.WriteByte((byte)(value >> 24));
            _stream.WriteByte((byte)((value >> 16) & 0xFF));
            _stream.WriteByte((byte)((value >> 8) & 0xFF));
            _stream.WriteByte((byte)(value & 0xFF));
        }

        /// <summary>
        /// Writes a signed 32-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt32(int value)
        {
            WriteUInt32((uint)value);
        }

        /// <summary>
        /// Writes an unsigned 64-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteUInt64(ulong value)
        {
            _stream.WriteByte((byte)(value >> 56));
            _stream.WriteByte((byte)((value >> 48) & 0xFF));
            _stream.WriteByte((byte)((value >> 40) & 0xFF));
            _stream.WriteByte((byte)((value >> 32) & 0xFF));
            _stream.WriteByte((byte)((value >> 24) & 0xFF));
            _stream.WriteByte((byte)((value >> 16) & 0xFF));
            _stream.WriteByte((byte)((value >> 8) & 0xFF));
            _stream.WriteByte((byte)(value & 0xFF));
        }

        /// <summary>
        /// Writes an signed 64-bit integer to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteInt64(long value)
        {
            WriteUInt64((ulong)value);
        }

        /// <summary>
        /// Writes a 32-bit float value to the underlying stream.
        /// </summary>
        /// <param name="value">The value to write.</param>
        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian)
            {
                // Is there a faster way to do this?
                byte temp = bytes[0];
                bytes[0] = bytes[3];
                bytes[3] = temp;
                temp = bytes[1];
                bytes[1] = bytes[2];
                bytes[2] = temp;
            }
            _stream.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes a UTF-16 encoded string to the underlying stream.
        /// </summary>
        /// <param name="str">The string to write.</param>
        public void WriteUTF16(string str)
        {
            foreach (char ch in str)
                WriteInt16((short)ch);
            WriteInt16(0x0000);
        }

        /// <summary>
        /// Writes an array of bytes to the underlying stream.
        /// </summary>
        /// <param name="data">The array of bytes to write.</param>
        public void WriteBlock(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
        }

        /// <summary>
        /// Changes the position of the underlying stream.
        /// </summary>
        /// <param name="offset">The new offset.</param>
        public void SeekTo(long offset)
        {
            _stream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        /// The stream that this BigEndianWriter is based off of.
        /// </summary>
        public Stream BaseStream
        {
            get { return _stream; }
        }

        private Stream _stream;
    }
}
