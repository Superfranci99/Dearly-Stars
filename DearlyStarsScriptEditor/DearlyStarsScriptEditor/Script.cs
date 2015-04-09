using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DearlyStarsScriptEditor
{
    public class Script
    {
        public string Path { get; set; }

        public Script(string path)
        {
            Path = path;
            FileStream fs = new FileStream(path, FileMode.Open);
            Read(fs);
        }


        // file structure properties
        SectionList    Sections  { get; set; }
        List<uint>     Pointers  { get; set; }
        public List<string>   Texts     { get; set; }
        public List<TextView> TextViews { get; set; }

        private void Read(FileStream fs)
        {
            BinReader br = new BinReader(fs);

            // read the header
            byte[] magicId  = br.ReadBytes(16);
            uint headerSize = br.ReadUInt32();
            uint nSections  = br.ReadUInt32();

            // if the header is longer, skip the other bytes
            // ... just to be sure
            if (br.BaseStream.Position != headerSize)
                br.BaseStream.Position = headerSize;

            // get section data
            this.Sections = new SectionList();
            for (int i = 0; i < nSections; i++)
            {
                Section sect = new Section();
                sect.Offset = (uint)br.BaseStream.Position;  // get section offset
                sect.Name = br.ReadUInt32();  // get section identifier
                
                // get all data from each section
                sect.Values = new uint[4];
                for (int x = 0; x < 4; x++)
                    sect.Values[x] = br.ReadUInt32();

                this.Sections.Add(sect);  // store the current section
            }

            // read pointer block
            this.Pointers = new List<uint>();
            br.BaseStream.Position = this.Sections.GetSection(7).Offset + this.Sections.GetSection(7).Values[0]; //section 7 at 0x4
            for (int i = 0; i < this.Sections.GetSection(7).Values[1]; i++)
                this.Pointers.Add(br.ReadUInt32());

            // read text block
            this.Texts = new List<string>();
            for (int i = 0; i < this.Sections.GetSection(7).Values[1]; i++)
            {
                br.BaseStream.Position =this.Sections.GetSection(7).Offset
                    + this.Sections.GetSection(7).Values[2] + this.Pointers[i];  // seek to next text
                this.Texts.Add(br.ReadJapString());
            }

            // get textview block
            int numTextViews = (int)this.Sections.GetSection(5).Values[3] / 16;
            this.TextViews = new List<TextView>(numTextViews);
            for (int i = 0; i < numTextViews; i++)
            {
                br.BaseStream.Position = this.Sections.GetSection(5).Offset
                + this.Sections.GetSection(5).Values[2] + i * 16;  // seek next textview

                TextView textview = new TextView();
                textview.Unknown1 = br.ReadByte();
                textview.Unknown2 = br.ReadByte();
                textview.Id = br.ReadUInt16();
                textview.IndexText1 = br.ReadInt32();
                textview.IndexText2 = br.ReadInt32();
                textview.IndexText3 = br.ReadInt32();

                this.TextViews.Add(textview);
            }

            // close filestream
            br.Close();
            fs.Close();
        }

        struct Section
        {
            public uint   Name   { get; set; }
            public uint   Offset { get; set; }
            public uint[] Values { get; set; }
        }

        public struct TextView
        {
            public byte Unknown1 { get; set; }
            public byte Unknown2 { get; set; }
            public ushort Id { get; set; }
            public int IndexText1 { get; set; }
            public int IndexText2 { get; set; }
            public int IndexText3 { get; set; }
        }



        // new classes
        class SectionList : List<Section>
        {
            public Section GetSection(int sectionName)
            {
                return this.Find(x => x.Name == sectionName);
            }

            public void Add(Section section)
            {
                if (!this.Exists(x => x.Name == section.Name))
                    base.Add(section);
                else
                    throw new NotSupportedException("There can't be sections with the same name");
            }            
        }

        class BinReader : BinaryReader
        {
            public BinReader(FileStream fs) : base(fs) { }

            public string ReadJapString()
            {
                return Encoding.GetEncoding("shift_jis").GetString(ReadUntilZero());
            }

            public byte[] ReadUntilZero()
            {
                List<byte> bytes = new List<byte>();
                do
                    bytes.Add(this.ReadByte());
                while (bytes[bytes.Count - 1] != 0);
                return bytes.ToArray();
            }
        }

    }

    
}
