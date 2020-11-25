using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharp.WebScripting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace ShowVersion
{
    public class ControlSystem : CrestronControlSystem
    {
        public HttpCwsServer myServer;
        private string procSerial = CrestronEnvironment.SystemInfo.SerialNumber;
        private string procFirmware = InitialParametersClass.FirmwareVersion;
        private string procHostname = Dns.GetHostName();
        private string procType = InitialParametersClass.ControllerPromptName;
        public string macAddress = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_MAC_ADDRESS, 0);
        public string ipAddress = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMATER_TO_GET.GET_CURRENT_IP_ADDRESS, 0);

        string cmdResponse = "";
        string versionResponse = "";
        string uptimeResponse = "";

        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 30;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(ControlSystem_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControlSystem_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(ControlSystem_ControllerEthernetEventHandler);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                myServer = new HttpCwsServer("/api");
                myServer.ReceivedRequestEvent += new HttpCwsRequestEventHandler(myServerReceivedRequestEvent);
                myServer.Routes.Add(new HttpCwsRoute("ver") { Name = "shversion" });
                myServer.Register();

                CrestronConsole.SendControlSystemCommand("progcomments:1", ref cmdResponse);
                CrestronConsole.SendControlSystemCommand("ver", ref versionResponse);
                CrestronConsole.SendControlSystemCommand("uptime", ref uptimeResponse);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        void myServerReceivedRequestEvent(object sender, HttpCwsRequestEventArgs args)
        {
            
            try
            {
                if (args.Context.Request.RouteData.Route.Name == "shversion")
                {
                    args.Context.Response.StatusCode = 200;
                    args.Context.Response.StatusDescription = "OK";
                    args.Context.Response.ContentType = "application/json";

                    string[] procVersion = versionResponse.Split(',');
                    string[] progComment = cmdResponse.Split('\n');
                    string[] procUptime = uptimeResponse.Split('\n');
                    string uptimeDays = procUptime[0].Replace("The system has been running for ", "");

                    // Create a simple JSON Object to send back to the frontend
                    // ( data:{ id:"",hostname:"",version:"",puf:"",serial:"",mac:"",systemname:"",compiled:"",uptime:"" } }
                    JObject resObject = new JObject
                    {
                        {
                            "data", new JObject
                            {
                                { "id", ipAddress },
                                { "hostname", procHostname },
                                { "version", procVersion[0].Split('[')[1] },
                                { "uptime", uptimeDays.Split('.')[0] },
                                { "puf", procFirmware },
                                { "serial", procSerial },
                                { "mac", macAddress },
                                { "systemname", progComment[3].Replace("\r", "").Replace("Program File: ", "") },
                                { "compiled", progComment[6].Replace("\r", "").Replace("Compiled On: ", "").Trim() }
                            }
                        }
                    };

                    string json = JsonConvert.SerializeObject(resObject);
                    args.Context.Response.Write(json, true);
                }
                else
                {
                    JObject errObject = new JObject
                    {
                        {
                            "data", new JObject
                            {
                                { "status", "error" },
                                { "msg", "Unhandled route" }
                            }
                        }
                    };
                    string jsonError = JsonConvert.SerializeObject(errObject);
                    args.Context.Response.Write(jsonError, true);
                }
            }
            catch (NullReferenceException e)
            {
                ErrorLog.Error("CwsServerNullError: {0}", e.Message);
            }
            catch (Exception e)
            {
                ErrorLog.Error("CwsServerError: {0}", e.Message);
                args.Context.Response.StatusCode = 404;
                args.Context.Response.Write("Error", true);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void ControlSystem_ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void ControlSystem_ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Stopping):
                    myServer.Dispose();
                    myServer.Unregister();
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void ControlSystem_ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.Rebooting):
                    myServer.Dispose();
                    myServer.Unregister();
                    break;
            }

        }
    }
}