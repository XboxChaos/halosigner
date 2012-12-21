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

namespace HaloSigner.Reach
{
    /// <summary>
    /// Provides methods for managing Halo: Reach campaign saves.
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

            Verify();
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

            Verify();
        }

        /// <summary>
        /// Verifies that the stream holds valid save data and throws an
        /// ArgumentException if it does not.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if the save data is invalid.</exception>
        private void Verify()
        {
            // Check the file size
            if (_stream.Length != 0xA70000)
            {
                throw new ArgumentException("Invalid save file");
            }

            // Check the header
            byte[] header = new byte[4];
            _stream.Position = 0;
            _stream.Read(header, 0, 4);
            if (header[0] != 0x60 || header[1] != 0xCC || header[2] != 0xCC || header[3] != 0xB2)
            {
                throw new ArgumentException("Invalid save file");
            }
        }

        /// <summary>
        /// Resigns the save data so that it can be read by the game.
        /// </summary>
        /// <seealso cref="HaloSigner.Reach.SaltedSHA1"/>
        public void Resign()
        {
            SaltedSHA1 hasher = new SaltedSHA1();

            // Fill the digest area with 0's and then hash the whole buffer
            _stream.Position = 0x1E708;
            _stream.Write(new byte[]{0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}, 0, 20);
            hasher.TransformFinalBlock(_stream.GetBuffer(), 0, _stream.GetBuffer().Length);
            
            // Write the new digest
            _stream.Position = 0x1E708;
            _stream.Write(hasher.Hash, 0, 20);
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
    }
}
