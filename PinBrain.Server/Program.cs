using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using log4net;

using PinBrain.Engine;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace PinBrain.Server
{
    class Program
    {
        protected static readonly new ILog _log = LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [STAThread()]
        static void Main(string[] args)
        {
            _log.Debug("Starting PinEngine.");
            Pingine engine = new Pingine();
            System.Windows.Forms.Application.Run();
            while (engine.IsRunning)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
