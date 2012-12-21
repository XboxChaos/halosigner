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
    /// Reads big-endian data from a stream.
    /// Designed to support the various types that Bungie uses.
    /// </summary>
    public class BigEndianReader
    {
        /// <summary>
        /// Constructs a new BigEndianReader object given a base stream.
        /// </summary>
        /// <param name="baseStream">The stream that data should be read from.</param>
        public BigEndianReader(Stream stream)
        {
            _stream = stream;
            _position = _stream.Position;
            _length = _stream.Length;
        }

        /// <summary>
        /// Closes this BigEndianReader and the underlying stream.
        /// </summary>
        public void Close()
        {
            _stream.Close();
        }

        /// <summary>
        /// Reads a byte from the underlying stream and advances its position by 1.
        /// </summary>
        public byte ReadByte()
        {
            _position++;
            return (byte)_stream.ReadByte();
        }

        /// <summary>
        /// Reads a signed byte from the underlying stream and advances its position by 1.
        /// </summary>
        public sbyte ReadSByte()
        {
            _position++;
            return (sbyte)_stream.ReadByte();
        }

        /// <summary>
        /// Reads a 16-bit unsigned integer from the underlying stream and advances its position by 2.
        /// </summary>
        public ushort ReadUInt16()
        {
            _position += 2;
            _stream.Read(_buffer, 0, 2);
            return (ushort)((_buffer[0] << 8) | _buffer[1]);
        }

        /// <summary>
        /// Reads a 16-bit signed integer from the underlying stream and advances its position by 2.
        /// </summary>
        public short ReadInt16()
        {
            _position += 2;
            _stream.Read(_buffer, 0, 2);
            return (short)((_buffer[0] << 8) | _buffer[1]);
        }

        /// <summary>
        /// Reads a 32-bit unsigned integer from the underlying stream and advances its position by 4.
        /// </summary>
        public uint ReadUInt32()
        {
            _position += 4;
            _stream.Read(_buffer, 0, 4);
            return (uint)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        /// <summary>
        /// Reads a 32-bit signed integer from the underlying stream and advances its position by 4.
        /// </summary>
        public int ReadInt32()
        {
            _position += 4;
            _stream.Read(_buffer, 0, 4);
            return (int)((_buffer[0] << 24) | (_buffer[1] << 16) | (_buffer[2] << 8) | _buffer[3]);
        }

        /// <summary>
        /// Reads a 64-bit unsigned integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public ulong ReadUInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (ulong)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                           (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            ulong upper = (ulong)ReadUInt32();
            ulong lower = (ulong)ReadUInt32();
            return (upper << 32) | lower;
        }

        /// <summary>
        /// Reads a 64-bit signed integer from the underlying stream and advances its position by 8.
        /// </summary>
        /// <returns>The value that was read</returns>
        public long ReadInt64()
        {
            /*_stream.Read(_buffer, 0, 8);
            return (long)((_buffer[0] << 56) | (_buffer[1] << 48) | (_buffer[2] << 40) | (_buffer[3] << 32) |
                          (_buffer[4] << 24) | (_buffer[5] << 16) | (_buffer[6] << 8) | _buffer[7]);*/
            return (long)ReadUInt64();
        }

        /// <summary>
        /// Reads a 32-bit float from the underlying stream and advances its position by 4.
        /// </summary>
        /// <returns>The float value that was read.</returns>
        public float ReadFloat()
        {
            _position += _stream.Read(_buffer, 0, 4);
            if (BitConverter.IsLittleEndian)
            {
                // Is there a faster way to do this?
                byte temp = _buffer[0];
                _buffer[0] = _buffer[3];
                _buffer[3] = temp;
                temp = _buffer[1];
                _buffer[1] = _buffer[2];
                _buffer[2] = temp;
            }
            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Changes the position of the underlying stream.
        /// </summary>
        /// <param name="offset">The new offset.</param>       
        public void SeekTo(long offset)
        {
            if (_position != offset)
            {
                _position = offset;
                _stream.Seek(_position, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Reads a null-terminated ASCII string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadAscii()
        {
            string result = "";
            int ch;
            while (true)
            {
                ch = _stream.ReadByte();
                _position++;
                if (ch == 0 || ch == -1)
                    break;
                result += (char)ch;
            }
            return result;
        }

        /// <summary>
        /// Reads a null-terminated UTF16-encoded string.
        /// </summary>
        /// <returns>The string that was read.</returns>
        public string ReadUTF16()
        {
            string result = "";
            int ch;
            while (true)
            {
                ch = (_stream.ReadByte() << 8) | _stream.ReadByte();
                _position += 2;
                if (ch == 0)
                {
                    // TODO: check for -1
                    break;
                }
                result += (char)ch;
            }
            return result;
        }

        public byte[] ReadBlock(int size)
        {
            byte[] result = new byte[size];
            _position += _stream.Read(result, 0, size);
            return result;
        }

        public void ReadBlock(byte[] output, int offset, int size)
        {
            _position += _stream.Read(output, offset, size);
        }

        /// <summary>
        /// Returns whether or not we are at the end of the stream.
        /// </summary>
        public bool EOF
        {
            get
            {
                return (_position >= _length);
            }
        }

        /// <summary>
        /// Returns the current position of the reader.
        /// </summary>
        public long Position
        {
            get
            {
                return _position;
            }
        }

        /// <summary>
        /// Returns the total length of the stream.
        /// </summary>
        public long Length
        {
            get
            {
                return _length;
            }
        }

        private Stream _stream = null;
        private byte[] _buffer = new byte[8];
        private long _position = 0;
        private long _length = 0;
    }
}
