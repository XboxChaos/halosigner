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
    /// The namespace types that GPD entries can use.
    /// </summary>
    public enum GPDNamespace : ushort
    {
        Achievement = 1,
        Image = 2,
        Setting = 3,
        Title = 4,
        String = 5,
        Security = 6,
        AvatarAward = 6
    }

    public static class GPDStringID
    {
        public const ulong TitleName = 0x8000;
    }

    /// <summary>
    /// Simple GPD class that's only useful for reading settings data.
    /// </summary>
    public class GPD : XDBF
    {
        /// <summary>
        /// Loads a GPD from a file.
        /// </summary>
        /// <param name="path">The path to the GPD file to load</param>
        public GPD(string path)
            : base(path)
        {
            try
            {
                ProcessEntries();
            }
            catch
            {
                Close();
                throw;
            }
        }

        /// <summary>
        /// Reads a GPD from a Stream.
        /// </summary>
        /// <param name="stream">The Stream to read from</param>
        public GPD(Stream stream)
            : base(stream)
        {
            ProcessEntries();
        }

        /// <summary>
        /// Finds a GPDSetting by ID.
        /// </summary>
        /// <param name="id">The setting ID to search for</param>
        /// <returns>The GPDSetting or null if not found.</returns>
        public GPDSetting FindSetting(uint id)
        {
            GPDSetting setting;
            if (_settings.TryGetValue(id, out setting))
                return setting;
            return null;
        }

        /// <summary>
        /// Finds a string by ID.
        /// </summary>
        /// <param name="id">The string ID to search for</param>
        /// <returns>The string or null if not found</returns>
        public string FindString(ulong id)
        {
            string str;
            if (_strings.TryGetValue(id, out str))
                return str;
            return null;
        }

        /// <summary>
        /// The name of the title that created this GPD (e.g. "Avatar Editor").
        /// It can be null if no title is stored.
        /// </summary>
        public string TitleName
        {
            get { return FindString(GPDStringID.TitleName); }
        }

        private void ProcessEntries()
        {
            EnumEntries(ProcessEntry);
        }

        private void ProcessEntry(XDBF container, XDBFEntry entry, BigEndianStream stream)
        {
            // Skip over sync lists
            if (entry.ID > 0xFFFFFFFF)
                return;

            switch ((GPDNamespace)entry.Namespace)
            {
                case GPDNamespace.Setting:
                    ReadSetting(entry, stream);
                    break;

                case GPDNamespace.String:
                    ReadString(entry, stream);
                    break;
            }
        }

        private void ReadSetting(XDBFEntry entry, BigEndianStream stream)
        {
            const long TypeOffset = 0x8;
            const long SettingHeaderSize = 0x10;

            stream.Position = entry.FileOffset;
            uint id = stream.ReadUInt32();
            stream.Position = entry.FileOffset + TypeOffset;
            GPDSettingType type = (GPDSettingType)stream.ReadByte();
            stream.Position = entry.FileOffset + SettingHeaderSize;

            GPDSetting setting;
            switch (type)
            {
                case GPDSettingType.Binary:
                    setting = new GPDBinarySetting(stream, id, type);
                    break;

                default:
                    setting = new GPDSetting(stream, id, type);
                    break;
            }
            _settings[id] = setting;
        }

        private void ReadString(XDBFEntry entry, BigEndianStream stream)
        {
            stream.Position = entry.FileOffset;
            _strings[entry.ID] = stream.ReadUTF16();
        }

        private Dictionary<uint, GPDSetting> _settings = new Dictionary<uint, GPDSetting>();
        private Dictionary<ulong, string> _strings = new Dictionary<ulong, string>();
    }
}
