using Gma.QrCodeNet.Encoding.Windows.Render;
using Gma.QrCodeNet.Encoding;
using QRCoder;
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
            using var generator = new QRCodeGenerator();
            using var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
            var qrCode = new PngByteQRCode(data);

            return qrCode.GetGraphic(pixelsPerModule: size / 25);
        }
    }
}
