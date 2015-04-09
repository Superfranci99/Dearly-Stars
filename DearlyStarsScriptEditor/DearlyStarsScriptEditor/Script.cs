using System;
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


        // file structure properties
        public List<Section> Sections { get; set; }



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

            // get section data
            this.Sections = new List<Section>();
            for (int i = 0; i < nSections; i++)
            {
                Section sect = new Section();
                sect.Offset = (uint)br.BaseStream.Position;  // get section offset
                sect.Name = br.ReadUInt32();  // get section identifier
                
                // get all data from each section
                sect.Values = new uint[4];
                for (int x = 0; x < 16; x++)
                    sect.Values[x] = br.ReadUInt32();

                this.Sections.Add(sect);  // store the current section
            }

            // read pointer block
            br.BaseStream.Position = this.Sections[6].Offset + this.Sections[6].Values[0];  //section 7 at 0x4
            for (int i = 0; i < this.Sections[6].Values[1]; i++)
            {
                //leggi puntatore e testo
            }
            

            br.Close();
            fs.Close();
        }

        struct Section
        {
            public uint   Name   { get; set; }
            public uint   Offset { get; set; }
            public uint[] Values { get; set; }
        }
    }
}
