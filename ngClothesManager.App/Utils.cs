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

        public static bool IsFileEqual(string path1, string path2) {
            using(var reader1 = new System.IO.FileStream(path1, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                using(var reader2 = new System.IO.FileStream(path2, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
                    byte[] hash1;
                    byte[] hash2;

                    using(var md51 = new System.Security.Cryptography.MD5CryptoServiceProvider()) {
                        md51.ComputeHash(reader1);
                        hash1 = md51.Hash;
                    }

                    using(var md52 = new System.Security.Cryptography.MD5CryptoServiceProvider()) {
                        md52.ComputeHash(reader2);
                        hash2 = md52.Hash;
                    }

                    int j = 0;
                    for(j = 0; j < hash1.Length; j++) {
                        if(hash1[j] != hash2[j]) {
                            break;
                        }
                    }

                    if(j == hash1.Length) {
                        return true;
                    } else {
                        return false;
                    }
                }
            }
        }

    }


}
