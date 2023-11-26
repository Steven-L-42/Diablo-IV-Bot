namespace PixelAimbot.Classes.Misc
{
    public static class Alert
    {
        public static void Show(string msg, FrmAlert.EnmType type = FrmAlert.EnmType.Success)
        {
            var frm = new FrmAlert();
            frm.TopMost = true;
            frm.ShowAlert(msg,type);
        }
    }
}