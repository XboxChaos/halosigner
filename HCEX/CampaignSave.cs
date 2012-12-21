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

namespace HaloSigner.HCEX
{
    /// <summary>
    /// Provides methods for managing HCEX campaign saves.
    /// </summary>
    public class CampaignSave
    {
        MemoryStream _stream = new MemoryStream();

        /// <summary>
        /// Constructs a new CampaignSave object based off of information in an
        /// mmiof.bmf file.
        /// </summary>
        /// <param name="mmiofBmfPath">The path to the mmiof.bmf file to load.</param>
        /// <exception cref="ArgumentException">Thrown if the file is an invalid mmiof.bmf file.</exception>
        public CampaignSave(string mmiofBmfPath)
        {
            FileStream fileStream = new FileStream(mmiofBmfPath, FileMode.Open, FileAccess.Read);
            _stream.SetLength(fileStream.Length);
            fileStream.Read(_stream.GetBuffer(), 0, (int)fileStream.Length);
            fileStream.Close();

            ReadData();
        }

        /// <summary>
        /// Constructs a new CampaignSave object based off of information in an
        /// mmiof.bmf file already loaded into a Stream.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <exception cref="ArgumentException">Thrown if the save data is invalid.</exception>
        public CampaignSave(Stream stream)
        {
            _stream.SetLength(stream.Length);
            stream.Read(_stream.GetBuffer(), 0, (int)stream.Length);

            ReadData();
        }

        /// <summary>
        /// Resigns the save data so that it can be read by the game.
        /// </summary>
        /// <seealso cref="HaloSigner.Reach.SaltedSHA1"/>
        public void Resign()
        {

        }

        /// <summary>
        /// Writes the modified save data to a new file.
        /// </summary>
        /// <param name="mmiofBmfPath">The path to the destination file.</param>
        public void WriteTo(string mmiofBmfPath)
        {
            FileStream fileStream = new FileStream(mmiofBmfPath, FileMode.Create, FileAccess.Write);
            fileStream.Write(_stream.GetBuffer(), 0, _stream.GetBuffer().Length);
            fileStream.Close();
        }

        /// <summary>
        /// Writes the modified save data to a Stream.
        /// </summary>
        /// <param name="stream">The Stream to write to.</param>
        public void WriteTo(Stream stream)
        {
            stream.Write(_stream.GetBuffer(), 0, _stream.GetBuffer().Length);
        }

        /// <summary>
        /// Processes the save contents once the stream has been loaded.
        /// </summary>
        private void ReadData()
        {
            Verify();
            ReadHeader();
        }

        /// <summary>
        /// Verifies that the stream holds valid save data and throws an
        /// ArgumentException if it does not.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the save data is invalid.</exception>
        private void Verify()
        {

        }

        /// <summary>
        /// Reads any information stored in the file header.
        /// </summary>
        private void ReadHeader()
        {

        }
    }
}
