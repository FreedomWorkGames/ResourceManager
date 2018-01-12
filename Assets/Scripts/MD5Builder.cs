using System.Text;
using System.Security.Cryptography;
using System.IO;

public class MD5Builder  {

	public static string BuildMD5(byte[] data)
    {
        MD5 md5 = MD5.Create();
        byte[] hashBytes = md5.ComputeHash(data);

        // Convert the byte array to hexadecimal string
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("X2"));
            // To force the hex string to lower-case letters instead of
            // upper-case, use he following line instead:
            // sb.Append(hashBytes[i].ToString("x2")); 
        }
        return sb.ToString();
    }

    public static string BuildMD5(string filePath)
    {
        FileStream file = new FileStream(filePath, System.IO.FileMode.Open);
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] retVal = md5.ComputeHash(file);
        file.Close();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < retVal.Length; i++)
        {
            sb.Append(retVal[i].ToString("X2"));
        }
        return sb.ToString();
    }
}
