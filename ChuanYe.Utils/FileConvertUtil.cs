using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Drawing;

namespace ChuanYe.Utils
{

    /// <summary>
    /// 注：调用时请注意以下备注
    /// 1.图片转换的工具类，该类库依赖：Magick.NET-AnyCPU.dll 这个c#库，
    /// 2.这个库（Magick.NET-AnyCPU.dll）又依赖于 vcomp110.dll, msvcp110.dll, msvcr110.dll（C:windows/system32/）vs运行时库
    /// </summary>
    public class FileConvertUtil
    {
        /// <summary>
        ///  将文件转换成Base64
        /// </summary>
        /// <param name="imageFileStream">流</param>
        /// <returns></returns>
        public static string ConvertToBase64(Stream fileStream)
        {
            string byte64String = string.Empty;
            BinaryReader br = new BinaryReader(fileStream);

            byte[] bytes = br.ReadBytes((Int32)fileStream.Length);
            byte[] buffer = new byte[fileStream.Length];

            fileStream.Read(buffer, 0, buffer.Length);
            fileStream.Close();
            byte64String = Convert.ToBase64String(bytes);
            return byte64String;
        }

        /// <summary>
        /// 将Base64编码文本转换成Byte[]
        /// </summary>
        /// <param name="base64">Base64编码文本</param>
        /// <returns></returns>
        public static Byte[] FromBase64ToByte(string base64)
        {
            char[] charBuffer = base64.ToCharArray();
            byte[] bytes = Convert.FromBase64CharArray(charBuffer, 0, charBuffer.Length);
            return bytes;
        }
    }
}
