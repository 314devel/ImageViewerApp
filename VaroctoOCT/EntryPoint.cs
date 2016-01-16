using System;


namespace VaroctoOCT
{

    public class Program
    {
        [STAThread]
        public static void Main()
        {
            VaroctoOCT.App app = new VaroctoOCT.App();
            app.InitializeComponent();
            app.Run();
        }
       
    }
}
