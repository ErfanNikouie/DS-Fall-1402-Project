using System;
using System.Text;
using System.Security.Cryptography;

public static class MD5Hash
{
    public static string CalculateHash(string s)
    {
        using MD5 md5 = MD5.Create();

        byte[] inputBytes = Encoding.ASCII.GetBytes(s);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        StringBuilder sb = new StringBuilder();
        foreach (byte t in hashBytes)
            sb.Append(t.ToString("X2"));

        return sb.ToString();
    }
}