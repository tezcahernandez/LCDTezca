using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace WSLocal
{
    public partial class Service1 : ServiceBase
    {
        Timer myTimer = null;
        public Service1()
        {
            InitializeComponent();
            myTimer = new System.Timers.Timer();
            myTimer.Interval = 5000;
            myTimer.Elapsed += new System.Timers.ElapsedEventHandler(myTimer_Elapsed);
        }
        
        void CallServicioWeb()
        {
            try
            {
                int hora = int.Parse(ConfigurationManager.AppSettings["hora"]);
                int minutos = int.Parse(ConfigurationManager.AppSettings["minutos"]);
                if (DateTime.Now.Hour == hora && DateTime.Now.Minute == minutos)
                {
                    WS.WSLCOSoapClient ws = new WS.WSLCOSoapClient();
                    int i = 0;
                    ws.UpdateLCO();
                    do
                    {
                        EventLog.WriteEntry("WSService EJECUTANDOSE " + DateTime.Now.ToString());
                        if (!ws.GetStatus()) ws.UpdateLCO();
                        System.Threading.Thread.Sleep(300000);
                        i++;
                    } while (i < 3);
                }
            }catch(Exception e){
                EventLog.WriteEntry("WSService ERROR " + e.Message);
            }
        }
        void myTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            myTimer.Enabled = false;
            CallServicioWeb();
            myTimer.Enabled = true;
        }
        protected override void OnStart(string[] args)
        {
            EventLog.WriteEntry("WSService iniciado");
            myTimer.Enabled = true;
        }

        protected override void OnStop()
        {
            myTimer.Enabled = false;
        }
    }
}
