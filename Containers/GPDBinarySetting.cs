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
    /// Represents a setting that stores binary data.
    /// </summary>
    public class GPDBinarySetting : GPDSetting
    {
        public GPDBinarySetting(BigEndianStream stream, uint id, GPDSettingType type)
            : base(stream, id, type)
        {
            const long SkipAmount = 4;

            _size = stream.ReadInt32();
            _dataOffset = stream.Position + SkipAmount;
        }

        public override BigEndianStream GetStream()
        {
            BigEndianStream stream = base.GetStream();
            stream.Position = _dataOffset;
            return stream;
        }

        /// <summary>
        /// The size of the binary data in bytes.
        /// </summary>
        public int Size
        {
            get { return _size; }
        }

        private int _size;
        private long _dataOffset;
    }
}
