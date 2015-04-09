using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DearlyStarsScriptEditor
{
    class Program
    {
        static void Main(string[] args)
        {
            Script sc = new Script(@"D:\Francesco\Documenti\GitHub\Dearly-Stars\Packs\FileData\system\154_AIH_GOP_MAIN01_MES.BBQ");
            
            BBQEditor editor = new BBQEditor(sc);
            editor.ShowDialog();
        }
    }
}
