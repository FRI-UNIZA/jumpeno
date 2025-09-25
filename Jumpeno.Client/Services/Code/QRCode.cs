namespace Jumpeno.Client.Services;

public static class QRCode {
    public static string SVG(string text) {
        // 1) Create generator:
        using var generator = new QRCoder.QRCodeGenerator();
        var qrCodeData = generator.CreateQrCode(text, QRCoder.QRCodeGenerator.ECCLevel.Q);
        // 2) Generate code:
        var svgQrCode = new QRCoder.SvgQRCode(qrCodeData);
        string svgContent = svgQrCode.GetGraphic(20, Color.Black, Color.White, false);
        // 3) Convert to a data URL format for embedding:
        return $"{CONTENT_TYPE.DATA_SVG},{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(svgContent))}";
    }
}
