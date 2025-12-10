namespace Jumpeno.Client.Utils;

public static class Translate {
    public static string Rounds(int i) {
        var count = Math.Abs(i);
        if (count == 0) return I18N.T("Rounds0");
        if (count <= 1) return I18N.T("Rounds1");
        if (count <= 4) return I18N.T("Rounds2+");
        return I18N.T("Rounds5+");
    }

    public static string Players(int i) {
        var count = Math.Abs(i);
        if (count == 0) return I18N.T("Players0");
        if (count <= 1) return I18N.T("Players1");
        if (count <= 4) return I18N.T("Players2+");
        return I18N.T("Players5+");
    }
}
