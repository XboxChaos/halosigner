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
    public enum GPDSettingType : byte
    {
        Context,
        Int32,
        Int64,
        Double,
        String,
        Float,
        Binary,
        DateTime
    }

    /// <summary>
    /// Represents a setting stored in a GPD file.
    /// </summary>
    public class GPDSetting
    {
        public GPDSetting(BigEndianStream stream, uint id, GPDSettingType type)
        {
            _stream = stream;
            _offset = stream.Position;
            _id = id;
            _type = type;
        }

        /// <summary>
        /// Returns a stream pointing to the start of this setting's data.
        /// </summary>
        public virtual BigEndianStream GetStream()
        {
            _stream.Position = _offset;
            return _stream;
        }

        /// <summary>
        /// The setting's ID
        /// </summary>
        public uint ID
        {
            get { return _id; }
        }

        /// <summary>
        /// The setting's type
        /// </summary>
        public GPDSettingType Type
        {
            get { return _type; }
        }

        private BigEndianStream _stream;
        private long _offset;
        private uint _id;
        private GPDSettingType _type;
    }
}
