using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace A7.Helpers
{
    public static class PictureHelper
    {
        /// <summary>
        /// BMP paveksliukas konvertuojamas į baitų masyvą atskiriant antraštę (tarnybinė informacija) ir kūną.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>Antraštė ir kūnas</returns>
        public static async Task<(byte[], byte[])> ReadBmpImageAsync(string filePath)
        {
            var byteArray = await File.ReadAllBytesAsync(filePath);
            var header = byteArray.Take(54).ToArray();
            var body = byteArray.Skip(54).ToArray();

            return (header, body);
        }

        /// <summary>
        /// Išsaugo BMP paveiksliuką.
        /// </summary>
        /// <param name="basePath"></param>
        /// <param name="pictureHeader"></param>
        /// <param name="pictureBody"></param>
        /// <param name="encoded"></param>
        /// <returns></returns>
        public static async Task WriteBmpImageAsync(string basePath, byte[] pictureHeader, byte[] pictureBody, bool encoded = true)
        {
            var ticks = DateTime.Now.Ticks;
            var pictureArray = pictureHeader.Concat(pictureBody).ToArray();
            var filepath = encoded == true ? Path.Combine(basePath, $"Picture-{ticks}-encoded.bmp")
                                           : Path.Combine(basePath, $"Picture-{ticks}.bmp");
            await File.WriteAllBytesAsync(filepath, pictureArray);
        }
    }
}
