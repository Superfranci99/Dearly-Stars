using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DearlyStarsScriptEditor
{
    public partial class BBQEditor : Form
    {
        public Script ScriptData { get; set; }

        public BBQEditor(Script script)
        {
            this.ScriptData = script;
            InitializeComponent();
        }

        private void BBQEditor_Load(object sender, EventArgs e)
        {
            numericUpDown1.Maximum = this.ScriptData.TextViews.Count - 1;
            numericUpDown1_ValueChanged(null, null);
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            int n = (int)numericUpDown1.Value;
            textBox1.Text = this.ScriptData.Texts[this.ScriptData.TextViews[n].IndexText1];
            textBox2.Text = this.ScriptData.Texts[this.ScriptData.TextViews[n].IndexText2];
            textBox3.Text = this.ScriptData.Texts[this.ScriptData.TextViews[n].IndexText3];
            textBox4.Text = "ID: " + this.ScriptData.TextViews[n].Id + "\r\n" +
                "Unk1: " + this.ScriptData.TextViews[n].Unknown1 + "\r\n" +
                "Unk2: " + this.ScriptData.TextViews[n].Unknown2;
        }
    }
}
