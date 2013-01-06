using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HaloSigner.Blam;

namespace HaloSigner.Halo3.ODST
{
    /// <summary>
    /// Provides methods for managing Halo: Reach campaign saves.
    /// </summary>
    public class CampaignSave
    {
        MemoryStream _stream = new MemoryStream();

        /// <summary>
        /// Constructs a new CampaignSave object based off of information in an
        /// mmiof.bmf file already loaded into a Stream.
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <exception cref="ArgumentException">Thrown if the save data is invalid.</exception>
        public CampaignSave(string mmiofBmfPath)
        {
            FileStream fileStream = new FileStream(mmiofBmfPath, FileMode.Open, FileAccess.Read);
            _stream.SetLength(fileStream.Length);
            fileStream.Read(_stream.GetBuffer(), 0, (int)fileStream.Length);
            fileStream.Close();
        }

        /// <summary>
        /// Resigns the save data so that it can be read by the game.
        /// </summary>
        /// <seealso cref="HaloSigner.Reach.SaltedSHA1"/>
        public void Resign()
        {
            SaltedSHA1 hasher = new SaltedSHA1();

            // Fill the digest area with 0's and then hash the whole buffer
            _stream.Position = 0xF138;
            _stream.Write(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 20);
            hasher.TransformFinalBlock(_stream.GetBuffer(), 0, _stream.GetBuffer().Length);

            // Write the new digest
            _stream.Position = 0xF138;
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
    }
}
