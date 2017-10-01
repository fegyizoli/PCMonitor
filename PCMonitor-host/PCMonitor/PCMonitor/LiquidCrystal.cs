using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace PCMonitor
{
    public class LiquidCrystal
    {
        private SerialPort port;
        
        private string line0;
        private string line1;
        private string line2;
        private string line3;

        public LiquidCrystal(SerialPort portref)
        {
            port = portref;
            line0 = "";
            line1 = "";
            line2 = "";
            line3 = "";
        }

        public void Connect()
        {
            byte[] cmd = { 0x2b }; /* + */
            port.Write(cmd, 0, 1);
        }

        public void Disconnect()
        {
            byte[] cmd = { 0x2d }; /* - */
            port.Write(cmd, 0, 1);
        }

        public void SetBacklight(uint value)
        {
            byte[] cmd = {0x5b, 0x42, (Convert.ToByte(value)) ,0x5d};
            port.Write(cmd, 0, cmd.Length);
        }

        public void ClearScreen()
        {
            byte[] cmd = { 0x5b, 0x43, 0x30, 0x5d };
            port.Write(cmd, 0, cmd.Length);
        }

        public void WriteLn(int line, string s)
        {
            int i, slen;
            byte[] cmd;

            string sdata = "[";
            
            switch(line)
            {
                case 0:
                {
                    sdata += "a";
                }
                break;
                
                case 1:
                {
                    sdata += "b";
                }
                break;
                
                case 2:
                {
                    sdata += "c";
                }
                break;
                
                case 3:
                {
                    sdata += "d";
                }
                break;
                
            }

            /* Fill unused characters with whitespace */
            if(s.Length < 20)
            {
                slen = 20 - s.Length;
                for (i = 0; i < slen; i++)
                {
                    s += " ";
                }
            }
            
            sdata += (s+"]");

            cmd = Encoding.ASCII.GetBytes(sdata);

            port.Write(cmd, 0, cmd.Length);
        }
        

        public void BuildScreen()
        {
            //ClearScreen();

            line0 = "Cpu: " + Math.Round((double)HardwareDescriptor.CpuTemp, 0).ToString() + "C (" + Math.Round((double)HardwareDescriptor.CpuTotalLoad, 0).ToString("00") + "%)";
            line1 = "Gpu: " + Math.Round((double)HardwareDescriptor.GpuTemp, 0).ToString() + "C (" + Math.Round((double)HardwareDescriptor.GpuLoad, 0).ToString("00") + "%)";
            line2 = "Gpu fan: " + Math.Round((double)HardwareDescriptor.GpuFan, 0).ToString("0000") + " rpm";
            line3 = "RAM: " + Math.Round((double)HardwareDescriptor.RamLoad, 0).ToString("00") + "% (" + HardwareDescriptor.RamFreeMem.ToString("0.0") + "G/" + HardwareDescriptor.RamUsedMem.ToString("0.0") + "G)";
            WriteLn(0, line0);
            WriteLn(1, line1);
            WriteLn(2, line2);
            WriteLn(3, line3);
        }
    }

}
