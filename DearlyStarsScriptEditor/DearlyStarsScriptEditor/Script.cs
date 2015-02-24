﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DearlyStarsScriptEditor
{
    class Script
    {
        public string Path { get; set; }

        public Script(string path)
        {
            Path = path;
            FileStream fs = new FileStream(path, FileMode.Open);
            Read(fs);
        }

        private void Read(FileStream fs)
        {
            BinaryReader br = new BinaryReader(fs);

            // read the header
            byte[] magicId  = br.ReadBytes(16);
            uint headerSize = br.ReadUInt32();
            uint nSections  = br.ReadUInt32();

            // if the header is longer, skip the other bytes
            // ... just to be sure
            if (br.BaseStream.Position != headerSize)
                br.BaseStream.Position = headerSize;

            // get 0-9 block offsets
            uint[] blockOffsets = new uint[9];
            for (int i = 0; i < nSections; i++)
            {
                br.BaseStream.Seek(headerSize + (i * 0x14), SeekOrigin.Begin);
                blockOffsets[br.ReadUInt16()] = (uint)br.BaseStream.Position - 2;
            }

            br.Close();
            fs.Close();
        }
    }
}