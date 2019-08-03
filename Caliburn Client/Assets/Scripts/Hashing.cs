using System;
using System.Text;
using System.Security.Cryptography;

public static class Hashing {
    public static string GetMd5Hash(MD5 md5Hash, string input) {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new StringBuilder();
        for(int i = 0; i < data.Length; i++) {
            sb.Append(data[i].ToString("x2"));
        }
        return sb.ToString();
    }

    public static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash) {
        string hashOfInput = GetMd5Hash(md5Hash, input);
        StringComparer comparer= StringComparer.OrdinalIgnoreCase;
        if(0 == comparer.Compare(hashOfInput, hash)) {
            return true;
        } else {
            return false;
        }
    }
}

