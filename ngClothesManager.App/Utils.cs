using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using RageLib.GTA5.Cryptography;
using RageLib.GTA5.Cryptography.Helpers;

namespace ngClothesManager.App {
    internal static class Utils {
        public static void EnsureKeys() {
            GTA5Constants.PC_AES_KEY = Resource.gtav_aes_key;
            GTA5Constants.PC_NG_KEYS = CryptoIO.ReadNgKeys(Resource.gtav_ng_key);
            GTA5Constants.PC_NG_DECRYPT_TABLES = CryptoIO.ReadNgTables(Resource.gtav_ng_decrypt_tables);
            GTA5Constants.PC_NG_ENCRYPT_TABLES = CryptoIO.ReadNgTables(Resource.gtav_ng_encrypt_tables);
            GTA5Constants.PC_NG_ENCRYPT_LUTs = CryptoIO.ReadNgLuts(Resource.gtav_ng_encrypt_luts);
            GTA5Constants.PC_LUT = Resource.gtav_hash_lut;
        }

        public static string GetRelativePath(string filespec, string folder) {
            Uri pathUri = new Uri(filespec);

            if(!folder.EndsWith(Path.DirectorySeparatorChar.ToString())) {
                folder += Path.DirectorySeparatorChar;
            }

            Uri folderUri = new Uri(folder);

            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
        }

        public static void HandleException(Exception exception) {
            MessageBox.Show(exception.Message);
        }

        public static char NumberToLetter(int number) {
            return (char)('a' + number);
        }
    }

    
}
