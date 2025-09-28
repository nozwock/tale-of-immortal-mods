using System;
using System.IO;
using System.Linq;

namespace MOD_swissTool
{
    internal class CryptFile
    {
        public static void Decrypt()
        {
            DataStruct<bool, string> path = OpenFileDialog.OpenFolder("Please select the Mod directory with files for Decryption.");
            if (!path.t1)
            {
                Console.WriteLine("No directory selected for decryption");
                return;
            }
            string decryptPath = path.t2;

            DataStruct<bool, string> topath = OpenFileDialog.OpenFolder("Please select the folder to save the decrypted files");
            if (!topath.t1)
            {
                Console.WriteLine("No folder selected to save decrypted files");
                return;
            }
            string savePath = topath.t2;

            var files = Directory.GetFiles(decryptPath, "*.json", SearchOption.AllDirectories)
                                .Union(Directory.GetFiles(decryptPath, "*.png", SearchOption.AllDirectories))
                                .Union(Directory.GetFiles(decryptPath, "*.cache", SearchOption.AllDirectories));
            foreach (string file in files)
            {
                UnhollowerBaseLib.Il2CppStructArray<byte> bytes = FileTool.ReadByte(file);
                if (IsEncrypt(bytes))
                {
                    var toBytes = EncryptTool.DecryptMult(bytes, GameConf.modEncryPassword);
                    var saveFile = file.Replace("\\", "/").Replace(decryptPath.Replace("\\", "/"), savePath);
                    var toDir = Path.GetDirectoryName(saveFile);
                    if (!Directory.Exists(toDir))
                    {
                        Directory.CreateDirectory(toDir);
                    }
                    Console.WriteLine(file + " >> " + saveFile);
                    FileTool.WriteByte(saveFile, toBytes);
                }
                else
                {
                    Console.WriteLine(file + " isn't encrypted, no need to decrypt.");
                }
            }
            Console.WriteLine("Decryption completed");
            UnityEngine.Application.OpenURL(savePath);
        }
        public static void Encrypt2()
        {
            DataStruct<bool, string> path = OpenFileDialog.OpenFolder("Please select the Mod directory with files for Encryption.");
            if (!path.t1)
            {
                Console.WriteLine("No directory selected for encryption");
                return;
            }
            string decryptPath = path.t2;

            DataStruct<bool, string> topath = OpenFileDialog.OpenFolder("Please select the folder to save the encrypted files");
            if (!topath.t1)
            {
                Console.WriteLine("No folder selected to save encrypted files");
                return;
            }
            string savePath = topath.t2;

            var files = Directory.GetFiles(decryptPath, "*.json", SearchOption.AllDirectories)
                                .Union(Directory.GetFiles(decryptPath, "*.png", SearchOption.AllDirectories))
                                .Union(Directory.GetFiles(decryptPath, "*.cache", SearchOption.AllDirectories));
            foreach (string file in files)
            {
                UnhollowerBaseLib.Il2CppStructArray<byte> bytes = FileTool.ReadByte(file);
                if (!IsEncrypt(bytes))
                {
                    var toBytes = EncryptTool.EncryptMult(bytes, GameConf.modEncryPassword);
                    var saveFile = file.Replace("\\", "/").Replace(decryptPath.Replace("\\", "/"), savePath);
                    var toDir = Path.GetDirectoryName(saveFile);
                    if (!Directory.Exists(toDir))
                    {
                        Directory.CreateDirectory(toDir);
                    }
                    Console.WriteLine(file + " >> " + saveFile);
                    FileTool.WriteByte(saveFile, toBytes);
                }
                else
                {
                    Console.WriteLine(file + " is already encrypted.");
                }
            }
            Console.WriteLine("Encryption completed");
            UnityEngine.Application.OpenURL(savePath);
        }

        public static void DecryptSav()
        {
            // Check if the game save directory exists
            if (!Directory.Exists(g.cache.cachePath))
            {
                Console.WriteLine($"Save directory doesn't exist: {g.cache.cachePath}");
                return;
            }
            Console.WriteLine($"Save directory is: {g.cache.cachePath}");
            string decryptPath = ModMain.savPath;
            // Select a folder to save the output
            DataStruct<bool, string> topath = OpenFileDialog.OpenFolder("Please select the folder to save the encrypted files");
            if (!topath.t1)
            {
                Console.WriteLine("No folder selected to save encrypted files");
                return;
            }
            string savePath = topath.t2;
            // Start decrypting and decompressing
            string[] files = Directory.GetFiles(decryptPath, "*", SearchOption.AllDirectories);
            string fileName; // Full filename field
            string Name; // Filename field without extension type
            foreach (string fileFullPath in files)
            {
                fileName = Path.GetFileName(fileFullPath);
                if (fileName.EndsWith(".cache"))
                { // Decrypt filename
                    Name = fileName.Replace(".cache", "");
                    fileName = fileName.Replace(Name, EncryptTool.DecryptDES(Name, GameConf.cacheEncryPassword));
                    Console.WriteLine($"{fileName}");
                }

                UnhollowerBaseLib.Il2CppStructArray<byte> bytes = FileTool.ReadByte(fileFullPath);
                if (IsEncrypt(bytes))
                {
                    var toBytes = CompressTool.Decompress(EncryptTool.DecryptMult(bytes, GameConf.cacheEncryPassword));
                    string saveFile = fileFullPath.Replace("\\", "/")           // Replace all backslashes in the file path with slashes
                        .Replace(decryptPath.Replace("\\", "/"), savePath)      // Replace decryptPath with savePath
                        .Replace(Path.GetFileName(fileFullPath), fileName);     // Replace encrypted filename with decrypted name
                    string toDir = Path.GetDirectoryName(saveFile); // Get the new Windows-stype path string to check its existence
                    if (!Directory.Exists(toDir))
                    {
                        Directory.CreateDirectory(toDir);
                    }
                    Console.WriteLine(fileFullPath + " >> " + saveFile);
                    FileTool.WriteByte(saveFile, toBytes);
                }
                else
                {
                    Console.WriteLine(fileFullPath + " isn't encrypted, no need to decrypt.");
                }
            }
            Console.WriteLine("Decryption completed");
            UnityEngine.Application.OpenURL(savePath);
        }

        // From an example-mod in the game's guide folder
        public static bool IsEncrypt(byte[] bytes)
        {
            string encryptKey = ModMain.valKey; // "2a;ad.,&fSf^SX.,:12@D"
            for (int i = 0; i < encryptKey.Length && i < bytes.Length; i++)
            {
                byte k = (byte)encryptKey[i];
                k = (byte)(k ^ 2 * 5 / 3);
                if (bytes[i] != k)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
