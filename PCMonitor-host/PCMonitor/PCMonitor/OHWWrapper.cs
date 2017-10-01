using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using OpenHardwareMonitor.Hardware;
using System.Windows.Forms;


namespace PCMonitor
{
    public class OHWWrapper
    {
        private Computer pc;

        public OHWWrapper()
        {
            pc = new Computer()
            {
                CPUEnabled = true,
                GPUEnabled = true,
                MainboardEnabled = true,
                HDDEnabled = true,
                RAMEnabled = true,
                FanControllerEnabled = false,
            
                /* FanControllerEnabled = true, */
            };
            pc.Open();
        }

        public void Init(ComboBox cb, SerialPort sp_ref)
        {
            try
            {
                sp_ref.Parity = Parity.None;
                sp_ref.StopBits = StopBits.One;
                sp_ref.DataBits = 8;
                sp_ref.Handshake = Handshake.None;
                sp_ref.RtsEnable = true;

                string[] ports = SerialPort.GetPortNames();
                foreach (string port_name in ports)
                {
                    cb.Items.Add(port_name);
                }

                sp_ref.BaudRate = 19200;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /* Get status of the hardwares */
        public void Status()
        {
            int i = 0;

            foreach(var hw in pc.Hardware)
            {
                /* Get Mainboard status */
                if(hw.HardwareType == HardwareType.Mainboard)
                {
                    HardwareDescriptor.MainboardName = hw.Name;
                }

                /* Get CPU status */
                if (hw.HardwareType == HardwareType.CPU)
                {
                    hw.Update();
                    HardwareDescriptor.CpuName = hw.Name;
                    HardwareDescriptor.CpuTotalLoad = hw.Sensors[0].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore0Load = hw.Sensors[1].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore1Load = hw.Sensors[2].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore2Load = hw.Sensors[3].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore3Load = hw.Sensors[4].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore0Clock = hw.Sensors[5].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore1Clock = hw.Sensors[6].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore2Clock = hw.Sensors[7].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuCore3Clock = hw.Sensors[8].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuTemp = hw.Sensors[9].Value.GetValueOrDefault();
                    HardwareDescriptor.CpuBusLoad = hw.Sensors[10].Value.GetValueOrDefault();
                }

                /* Get RAM status */
                if (hw.HardwareType == HardwareType.RAM)
                {
                    hw.Update();
                    HardwareDescriptor.RamName = hw.Name;
                    HardwareDescriptor.RamLoad = hw.Sensors[0].Value.GetValueOrDefault();
                    HardwareDescriptor.RamUsedMem = hw.Sensors[1].Value.GetValueOrDefault();
                    HardwareDescriptor.RamFreeMem = hw.Sensors[2].Value.GetValueOrDefault();
                }
                
                /* Get GPU status */
                if(hw.HardwareType == HardwareType.GpuAti)
                {
                    hw.Update();
                    HardwareDescriptor.GpuName = hw.Name;
                    HardwareDescriptor.GpuTemp = hw.Sensors[0].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuFan = hw.Sensors[1].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuFanCtrl = hw.Sensors[2].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuCoreClock = hw.Sensors[3].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuMemClock = hw.Sensors[4].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuVolts = hw.Sensors[5].Value.GetValueOrDefault();
                    HardwareDescriptor.GpuLoad = hw.Sensors[6].Value.GetValueOrDefault();
                }


                
                /* Get SSD/HDD statuses */
                if(hw.HardwareType == HardwareType.HDD)
                {
                    switch(i)
                    {
                        /* Get System SSD status */
                        case 0:
                        {
                            HardwareDescriptor.SsdSysName = hw.Name;
                            HardwareDescriptor.SsdSysTemp = hw.Sensors[0].Value.GetValueOrDefault();
                            HardwareDescriptor.SsdSysLoad = hw.Sensors[1].Value.GetValueOrDefault();
                            i++;
                        }
                        break;

                        /* Get Gamer SSD status */
                        case 1:
                        {
                            HardwareDescriptor.SsdGameName = hw.Name;
                            HardwareDescriptor.SsdGameLoad = hw.Sensors[0].Value.GetValueOrDefault();
                            i++;
                        }
                        break;

                        /* Get HDD status */
                        case 2:
                        {
                            HardwareDescriptor.HddName = hw.Name;
                            HardwareDescriptor.HddTemp = hw.Sensors[0].Value.GetValueOrDefault();
                            HardwareDescriptor.HddLoad = hw.Sensors[1].Value.GetValueOrDefault();
                            i++;
                        }
                        break;
                        
                        /* Last drive (don't care) */
                        case 6:
                        {
                            i = 0;
                        }
                        break;
                    }
                }
                
            }
            /*End of main foreach */
        }


        /* Build treeview up */
        public void updateTreeView(TreeView tv)
        {
            tv.Nodes.Clear();

            foreach(var hw in pc.Hardware)
            {
                if(hw.HardwareType == HardwareType.Mainboard)
                {
                    getHardware(tv, hw, HardwareType.Mainboard);
                }
                if (hw.HardwareType == HardwareType.SuperIO)
                {
                    getHardware(tv, hw, HardwareType.SuperIO);
                }
                if (hw.HardwareType == HardwareType.CPU)
                {
                    getHardware(tv, hw, HardwareType.CPU);
                }
                if (hw.HardwareType == HardwareType.GpuAti)
                {
                    getHardware(tv, hw, HardwareType.GpuAti);
                }
                if (hw.HardwareType == HardwareType.RAM)
                {
                    getHardware(tv, hw, HardwareType.RAM);
                }
                if (hw.HardwareType == HardwareType.HDD)
                {
                    getHardware(tv, hw, HardwareType.HDD);
                }
            }
            tv.ExpandAll();
        }

        public void setGPUFanspeed(IHardware hw, int value)
        {
            if(hw.HardwareType == HardwareType.GpuAti)
            {
                hw.Update();
                foreach(var sensor in hw.Sensors)
                {
                    if(sensor.SensorType == SensorType.Control)
                    {
                        
                    }
                }
            }
        }
        
        public void getHardware(TreeView tv_ref, IHardware hw, HardwareType hwtype)
        {
            List<TreeNode> tlist = new List<TreeNode>();

            string hwname = "Empty hardware";

            switch(hwtype)
            {
                case HardwareType.CPU:
                {
                    hwname = "CPU: "+hw.Name;
                }
                break;

                case HardwareType.GpuAti:
                {
                    hwname = "GPU: " + hw.Name;
                }
                break;

                case HardwareType.HDD:
                {
                    hwname = "HDD: " + hw.Name;
                }
                break;

                case HardwareType.Mainboard:
                {
                    hwname = "Mainboard: " + hw.Name;
                }
                break;
                
                case HardwareType.SuperIO:
                {
                    hwname = "IO: " + hw.Name;
                }
                break;

                case HardwareType.RAM:
                {
                    hwname = "RAM: " + hw.Name;
                }
                break;
            }

            hw.Update();
            foreach(var sensor in hw.Sensors)
            {
                tlist.Add(new TreeNode(sensor.SensorType.ToString()+": value: "+sensor.Value.GetValueOrDefault()));
            }
            

            tv_ref.Nodes.Add(new TreeNode(hwname, tlist.ToArray()));
        }

    }

    public enum HardwareProp
    {
        MainBoardName,

        CpuName,
        CpuTotalLoad,
        CpuCore0Load,
        CpuCore1Load,
        CpuCore2Load,
        CpuCore3Load,
        CpuCore0Clock,
        CpuCore1Clock,
        CpuCore2Clock,
        CpuCore3Clock,
        CpuTemp,
        CpuBusLoad,

        RamName,
        RamLoad,
        RamUsedMem,
        RamFreeMem,

        GpuName,
        GpuTemp,
        GpuFan,
        GpuFanCtrl,
        GpuCoreClock,
        GpuMemClock,
        GpuVolts,
        GpuLoad,

        SsdSysName,
        SsdSysTemp,
        SsdSysLoad,

        SsdGameName,
        SsdGameLoad,

        HddName,
        HddTemp,
        HddLoad
    }
}
