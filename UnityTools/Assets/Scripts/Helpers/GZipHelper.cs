using System;
using System.IO;
using System.Text;
using Ionic.BZip2;

[System.Serializable]
public class GZipHelper
{
    public static string Compress(string input)
    {
        using (var outputMemoryStream = new MemoryStream())
        {
            using (var zipStream = new BZip2OutputStream(outputMemoryStream))
            {
                var inputBytes = Encoding.UTF8.GetBytes(input);
                zipStream.Write(inputBytes, 0, inputBytes.Length);
            }

            var outputBytes = outputMemoryStream.ToArray();
            return Convert.ToBase64String(outputBytes);
        }
    }

    public static string Decompress(string input)
    {
        using (var inputMemoryStream = new MemoryStream(Convert.FromBase64String(input)))
        {
            using (var zipStream = new BZip2InputStream(inputMemoryStream))
            {
                using (var outputMemoryStream = new MemoryStream())
                {
                    int n = 0;
                    var buffer = new byte[2048];
                    while ((n = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        outputMemoryStream.Write(buffer, 0, n);
                    }

                    var outputBytes = outputMemoryStream.ToArray();
                    return Encoding.UTF8.GetString(outputBytes, 0, outputBytes.Length);
                }
            }
        }
    }
}