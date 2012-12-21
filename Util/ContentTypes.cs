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

namespace HaloSigner.Util
{
    public enum ContentType
    {
        Unknown,
        ReachGPD,
        ReachCampaignSave,
        Halo3CampaignSave,
        Halo3ODSTCampaignSave,
        HCEXCampaignSave
    }

    public static class ContentTypes
    {
        public static ContentType CheckFileType(Stream stream)
        {
            const int StandardHeaderSize = 4;
            const int HCEXHeaderSize = 0x18;

            byte[] header = new byte[StandardHeaderSize];
            stream.Read(header, 0, StandardHeaderSize);

            // First, try checking the header
            if (header[0] == 0x58 && header[1] == 0x44 && header[2] == 0x42 && header[3] == 0x46)
                return ContentType.ReachGPD;
            else if (header[0] == 0x60 && header[1] == 0xCC && header[2] == 0xCC && header[3] == 0xB2)
                return ContentType.ReachCampaignSave;
            else if (header[0] == 0x4E && header[1] == 0xB2 && header[2] == 0xC1 && header[3] == 0x86)
                return ContentType.Halo3CampaignSave;
            else if (header[0] == 0x4C && header[1] == 0x7D && header[2] == 0xEB && header[3] == 0x9F)
                return ContentType.Halo3ODSTCampaignSave;

            if (stream.Length < 0x40A018)
                return ContentType.Unknown;

            // CEA saves don't have a clearly-defined header
            stream.Position = stream.Length - 0x40A000;
            header = new byte[HCEXHeaderSize];
            stream.Read(header, 0, HCEXHeaderSize);

            string compressionStr = Encoding.ASCII.GetString(header, 0, 0x13);
            if (compressionStr == "non compressed save" && header[0x14] == 0x92 && header[0x15] == 0xF7 && header[0x16] == 0xE1 && header[0x17] == 0x04)
                return ContentType.HCEXCampaignSave;
            
            return ContentType.Unknown;
        }
    }
}
