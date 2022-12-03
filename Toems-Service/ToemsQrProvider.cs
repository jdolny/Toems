using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwoFactorAuthNet.Providers.Qr;

namespace Toems_Service
{
    internal class ToemsQrProvider : IQrCodeProvider
    {
        public string GetMimeType()
        {
            return "image/png";
        }

        public byte[] GetQrCodeImage(string text, int size)
        {
            var encoder = new QrEncoder();
            var qrCode = encoder.Encode(text);

            var renderer = new GraphicsRenderer(new FixedCodeSize(size, QuietZoneModules.Two));
            byte[] result;
            using (var stream = new MemoryStream())
            {
                renderer.WriteToStream(qrCode.Matrix, System.Drawing.Imaging.ImageFormat.Png, stream);
                result = stream.ToArray();
            }

            return result;
        }
    }
}
