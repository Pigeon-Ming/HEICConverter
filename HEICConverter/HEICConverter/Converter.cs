using ImageMagick;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HEICConverter
{
    public class Converter
    {
        //public static async Task jpg2HEIC(string FilePath)
        //{
        //    if (String.IsNullOrEmpty(FilePath))
        //        return;
        //    string jpgFilePath = FilePath;
        //    string heicFilePath = FilePath.Replace(".jpg", ".HEIC").Replace(".JPG", ".HEIC");

        //    // 使用MagickImage加载HEIC文件
        //    using (MagickImage image = new MagickImage(jpgFilePath))
        //    {
        //        // 设置输出格式为JPG
        //        image.Format = MagickFormat.Heic;

        //        // 保存为JPG文件
        //        await image.WriteAsync(heicFilePath);
        //    }

        //    Console.WriteLine("转换完成！");
        //}
        public static async Task HEIC2jpg(string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath))
                return;
            string heicFilePath = FilePath;
            string jpgFilePath = FilePath.Replace(".heic",".jpg").Replace(".HEIC",".jpg");

            // 使用MagickImage加载HEIC文件
            using (MagickImage image = new MagickImage(heicFilePath))
            {
                // 设置输出格式为JPG
                image.Format = MagickFormat.Jpg;

                // 保存为JPG文件
                await image.WriteAsync(jpgFilePath);
            }

            Console.WriteLine("转换完成！");
        }

        public static async Task HEIC2png(string FilePath)
        {
            if (String.IsNullOrEmpty(FilePath))
                return;
            string heicFilePath = FilePath;
            string pngFilePath = FilePath.Replace(".heic", ".png").Replace(".HEIC", ".png");

            // 使用MagickImage加载HEIC文件
            using (MagickImage image = new MagickImage(heicFilePath))
            {
                // 设置输出格式为PNG
                image.Format = MagickFormat.Png;

                // 保存为PNG文件
                await image.WriteAsync(pngFilePath);
            }

            Console.WriteLine("转换完成！");
        }

    }
}
