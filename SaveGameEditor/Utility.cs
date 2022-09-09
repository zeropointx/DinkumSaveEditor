using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SaveGameEditor
{
    public class Utility
    {
        public static object load(FileStream fileStream)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object o = binaryFormatter.Deserialize(new BufferedStream(fileStream));
            fileStream.Close();
            return o;
        }

        public static object load(FileStream fileStream, ModuleDefinition def)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            object o = binaryFormatter.Deserialize(new BufferedStream(fileStream));
            fileStream.Close();
            return o;
        }
    }
}
