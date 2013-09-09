using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace HolidayTest1
{
    public partial class MainForm
    {
        // This version of BuildPortList only enumerates FTDI serial ports
        private void BuildPortList(ComboBox combo)
        {
            string[] dev = new string[0];   // in case registry key not found
            RegistryKey rk1 = null;
            RegistryKey rk2 = null;
            int count = 0, num, high = 0, pos;

            combo.Items.Clear();

            // HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FTSER2K\\Enum
            try
            {
                rk1 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\FTSER2K\\Enum");
                count = (int)rk1.GetValue("Count");
            }
            catch (Exception) { } // ex) { MessageBox.Show(ex.Message); }

            if (rk1==null || count<=0) return;
 
            //MessageBox.Show(dev.Length.ToString());
            for (int n = 0; n < count; n++)
            {
                try
                {
                    string drv = (string)rk1.GetValue(n.ToString());
                    if (drv == null) continue;

                    // HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\<devname>\\Device Parameters
                    rk2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                    rk2 = rk2.OpenSubKey(drv);
                    rk2 = rk2.OpenSubKey("Device Parameters");
                    if (rk2 == null) continue;

                    string port = (string)rk2.GetValue("PortName");
                    if (drv == null) continue;

                    if (drv.StartsWith("FTDIBUS\\VID_0403+PID_6001+") && port.Length > 3 && port.StartsWith("COM"))
                    {
                        //MessageBox.Show(port + " " + drv);
                        //if (debug>1) AddLog("Found " + port);
                        
                        num = Int32.Parse(port.Substring(3));

                        // work out position to include in list to keep proper numeric order (not alpha sort)
                        pos = -1;
                        for (int i = 0; i < combo.Items.Count; i++)
                        {
                            if (Int32.Parse(combo.Items[i].ToString().Substring(3)) > num)
                            {
                                pos = i;
                                break;
                            }
                        }

                        // insert or add to list, but ignore if port number invalid
                        if (num == 0)
                            continue;
                        else if (pos < 0)
                            combo.Items.Add(port);
                        else
                            combo.Items.Insert(pos, port);
                        //if (debug>1) AddLog("COM" + num.ToString() + " max=" + high.ToString() + " pos=" + pos.ToString());

                        // default to highest port found - with USB devices this is likely
                        if (num > high)
                        {
                            high = num;
                            combo.Text = port;
                        }
                    }
                }
                catch (Exception) { } // ex) { MessageBox.Show(ex.Message); }
            }
        }

        // Search for specified COM port, and remove "Upper Filters"="serenum" if it exists
        // This attribute causes "serial enumeration" on the COM port to detect plug and play devices like serial mice
        // The serial enumeration toggles RTS/CTS in undesirable ways
        // "Upper Filters" is a REG_MULTI_SZ - so we currently just nuke it completely
        // this probably won't work unless run with admin rights
        string RemoveFTDISerEnum(string port)
        {
            string[] dev = new string[0];   // in case registry key not found
            RegistryKey rk1 = null;
            RegistryKey rk2 = null;
            RegistryKey rk3 = null;
            int count = 0;

            // HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Services\\FTSER2K\\Enum
            try
            {
                rk1 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\FTSER2K\\Enum");
                count = (int)rk1.GetValue("Count");
            }
            catch (Exception) { } // ex) { MessageBox.Show(ex.Message); }

            if (rk1 == null || count <= 0) return "Couldn't access registry data for FTDI";

            //MessageBox.Show(dev.Length.ToString());
            for (int n = 0; n < count; n++)
            {
                try
                {
                    string drv = (string)rk1.GetValue(n.ToString());
                    if (drv == null) continue;

                    // HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Enum\\<devname>\\Device Parameters
                    rk2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                    rk2 = rk2.OpenSubKey(drv);
                    rk3 = rk2.OpenSubKey("Device Parameters");
                    if (rk3 == null) continue;

                    if (drv.StartsWith("FTDIBUS\\VID_0403+PID_6001+") && port.Equals((string)rk3.GetValue("PortName")))
                    {
                        try
                        {
                            rk2.DeleteValue("Upper Filters");
                            return "Upper Filters setting removed on " + port;
                        }
                        catch (Exception ex) {
                            return "Error removing Upper Filters setting on " + port + "\r\n" + ex.Message;
                        }
                    }
                }
                catch (Exception) { } // ex) { MessageBox.Show(ex.Message); }
            }
            return "Could not find port " + port;
        }
    }
}
