#define DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DearlyStarsScriptEditor
{
    class Program
    {   
        [STAThread]
        static void Main(string[] args)
        {
            // when debugging open a file from a certain path
            Script sc = null;
#if DEBUG
            sc = new Script(@"D:\Francesco\Documenti\GitHub\Dearly-Stars\Packs\FileData\system\154_AIH_GOP_MAIN01_MES.BBQ");
#else
            OpenFileDialog op = new OpenFileDialog();
            if (op.ShowDialog() == DialogResult.OK)
                sc = new Script(op.FileName);
            else
                Application.Exit();
#endif
            
            BBQEditor editor = new BBQEditor(sc);
            editor.ShowDialog();
        }
    }
}
