using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Microsoft.Win32;

namespace HolidayTest1
{
    public partial class MainForm : Form
    {
        private const String defaultdb = "Data\\holitest.s3db";
        private const String readonlydb = "Data\\holitest.s3db;Read Only=True";
        
        bool allowserialskip = false;

        // registry key for settings
        static string keyname = "HKEY_CURRENT_USER\\Software\\MooresCloud";

        const string testcolour = "05a55f"; // weird cyan colour - chosen for the bit pattern

        enum STATE
        {
            IDLE, BEGIN, POWERON, BOOTLOAD, CHECKOTP, PROGOTP, CHECKRAM, AVRDUDE, CHECKATMEGA,
            CHECKANALOG, CHECKVOLTAGE, CHECKBUTTONS, ERASEEEPROM, PREPEEPROM, CHECKEEPROM, CHECKSPI,
            ALLGOOD, POWEROFF, END
        };

        // if you change this list, also edit ITEMNAME array below and review Process() state machine
        enum ITEM
        {
            POWERON, CHECKCOM, BOOTLOAD, CHECKOTP, PROGOTP, PRINTLABEL, CHECKRAM, AVRDUDE,
            CHECKATMEGA, CHECKVOLTAGE, CHECKBUTTONS, CHECKANALOG, ERASEEEPROM, PREPEEPROM, CHECKEEPROM,
            CHECKSPI
        };

        string[] ITEMNAME =
        {
            "Power Test",
            "Check imx communications",
            "Bootload imx test firmware",
            "Check imx OTP/serial number",
            "Program imx OTP/serial number",
            "Print label",
            "Check RAM",
            "Program ATmega",
            "Check ATmega communications",
            "Check Supply Voltage (estimate)",
            "Check Buttons (are off)",
            "Check Analog",
            "Erase I2C EEPROM (page 0)",
            "Prepare I2C EEPROM",
            "Verify I2C EEPROM",
            "Test SPI"
        };

        enum IMAGE { NONE, HERE, PASS, FAIL, RETRY, INFO, STAR, WARN, RESTART, POWER, PLUS, MINUS };

        bool loading = true;
        string rcv = "";
        STATE state;
        bool endtest = false;
        int timeout = 0;
        int exitflag = 0;
        int subitem = 0;
        int retry = 0;
        uint snprefix = 0;
        int stationnum = 1;
        uint serialnum = 0;
        string serialhex = "";
        bool printonly = false;
        bool otpupdate = false;
        DateTime teststart = DateTime.Now;

        System.Diagnostics.Process sbloader = null;
        System.Diagnostics.Process avrdude = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            labelVersion.Text = "Version: " + Application.ProductVersion;

            try
            {
                stationnum = (int)Registry.GetValue(keyname, "TestStationNum", stationnum);
            }
            catch (Exception) { /*AddLogErr("Test Station Number not available: " + ex.Message);*/ }
            AddLog("Test Station # = " + stationnum.ToString());
            comboBoxStationNum.Text = stationnum.ToString();

            SetupTestList();

            BuildPortList(comboBoxPort);

            listViewLog_Resize(null, null);
            listViewStatus_Resize(null, null);

            loading = false;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            AboutForm f = new AboutForm();
            f.ShowDialog();
        }

        delegate void EnableUICallback(bool f);

        void EnableUI(bool f)
        {
            if (this.comboBoxPort.InvokeRequired)
            {
                EnableUICallback d = new EnableUICallback(this.EnableUI);
                this.BeginInvoke(d, f);
            }
            else
            {
                buttonBegin.Enabled = f;
                buttonRePrint.Enabled = f;
                buttonNewBatch.Enabled = f;
                comboBoxPort.Enabled = f;
                comboBoxStationNum.Enabled = f;
            }
        }

        void SetupTestList()
        {
            listViewStatus.Items.Clear();
            for (int i = 0; i < ITEMNAME.Length; i++)
                listViewStatus.Items.Add((i + 1).ToString() + ". " + ITEMNAME[i]);
        }

        delegate void EndTestCallback();

        private void EndTest()
        {
            if (this.buttonBegin.InvokeRequired)
            {
                EndTestCallback d = new EndTestCallback(this.EndTest);
                this.BeginInvoke(d);
            }
            else
            {
                timer1.Stop();

                KillProcesses();

                serialPort1.DtrEnable = false;
                serialPort1.RtsEnable = false;
                if (serialPort1.IsOpen) serialPort1.Close();

                AddLog("Total test time = " + DateTime.Now.Subtract(teststart).Seconds.ToString() + " seconds"); 

                buttonBegin.Text = "Begin Test";
                buttonBegin.ForeColor = Color.DarkGreen;

                buttonGetNext_Click(null, null);

                EnableUI(true);
            }
        }

        private void buttonBegin_Click(object sender, EventArgs e)
        {
            if (buttonBegin.Text.StartsWith("STOP"))
            {
                ShowStateErr("Test cancelled");
                SetState(STATE.POWEROFF);
            }
            else
            {
                if (snprefix==0)
                {
                    MessageBox.Show("Please click 'Begin New Batch' button to start serial number allocation");
                    return;
                }

                EnableUI(false);
                SetupTestList();

                printonly = false;
                BeginTests();
            }
        }

        private void buttonRePrint_Click(object sender, EventArgs e)
        {
            printonly = true;
            BeginTests();
            //ItemState(ITEM.PRINTLABEL, IMAGE.HERE);
            //PrintLabel(textBoxSerial.Text);
        }

        void BeginTests()
        {
            EnableUI(false);
            SetupTestList();

            buttonBegin.Text = "STOP TEST";
            buttonBegin.ForeColor = Color.Red;
            buttonBegin.Enabled = true;

            ShowState("Starting test");
            ShowSerial("");

            listViewLog.Items.Clear();
            endtest = false;

            teststart = DateTime.Now;
            if (printonly)
                AddLog("Label Print started " + teststart.ToLongDateString() + " " + teststart.ToLongTimeString());
            else
                AddLog("Test started " + teststart.ToLongDateString() + " " + teststart.ToLongTimeString());

            serialnum = 0;
            serialhex = "";
            otpupdate = false;

            if (serialPort1.IsOpen) serialPort1.Close();

            KillProcesses();
            
            // this probably won't work unless run with admin rights
            RemoveFTDISerEnum(comboBoxPort.Text);

            SetState(STATE.BEGIN);

            timer1.Interval = 100;
            timer1.Start();
        }

        void SetState(STATE s, int to)
        {
            state = s;
            timeout = to;
            retry = 0;
        }

        void SetState(STATE s)
        {
            SetState(s, 100);
        }

        delegate void AddLogCallback(string text, Color hilite);

        void AddLog(string text)
        {
            AddLog(text, Color.White);
        }

        void AddLogErr(string text)
        {
            AddLog(text, Color.Yellow);
        }

        void AddLogGood(string text)
        {
            AddLog(text, Color.LightGreen);
        }

        void AddLog(string text, Color hilite)
        {
            if (this.listViewLog.InvokeRequired)
            {
                AddLogCallback d = new AddLogCallback(this.AddLog);
                this.BeginInvoke(d, text, hilite);
            }
            else
            {
                listViewLog.Items.Add(text);
                listViewLog.Items[listViewLog.Items.Count - 1].BackColor = hilite;

                // remove older entries
                while (listViewLog.Items.Count > 1000)
                    listViewLog.Items.RemoveAt(0);

                listViewLog.EnsureVisible(listViewLog.Items.Count - 1);
            }
        }

        delegate void ReplaceLogCallback(string check, string text);

        void ReplaceLog(string check, string text)
        {
            if (this.listViewLog.InvokeRequired)
            {
                ReplaceLogCallback d = new ReplaceLogCallback(this.ReplaceLog);
                this.BeginInvoke(d, check, text);
            }
            else
            {
                if (listViewLog.Items.Count > 0 &&
                    listViewLog.Items[listViewLog.Items.Count - 1].Text.StartsWith(check))
                    listViewLog.Items[listViewLog.Items.Count - 1].Text = text;
                else
                    listViewLog.Items.Add(text);

                listViewLog.EnsureVisible(listViewLog.Items.Count - 1);
            }
        }

        delegate void ShowStateCallback(string text, Color c);

        void ShowState(string text)
        {
            ShowState(text, Color.Black);
            AddLog("Status: " + text);
        }
        void ShowStateGood(string text)
        {
            ShowState(text, Color.Green);
            AddLogGood("Good: " + text);
        }
        void ShowStateErr(string text)
        {
            ShowState(text, Color.Red);
            AddLogErr("Error: " + text);
        }

        void ShowState(string text, Color c)
        {
            if (this.labelState.InvokeRequired)
            {
                ShowStateCallback d = new ShowStateCallback(this.ShowState);
                this.BeginInvoke(d, text, c);
            }
            else
            {
                labelState.Text = text;
                labelState.ForeColor = c;
            }
        }

        delegate void ItemStateCallback(ITEM n, IMAGE v);

        void ItemState(ITEM n, IMAGE v)
        {
            if (this.listViewStatus.InvokeRequired)
            {
                ItemStateCallback d = new ItemStateCallback(this.ItemState);
                this.BeginInvoke(d, n, v);
            }
            else
            {
                if (listViewStatus.Items[(int)n].StateImageIndex != (int)v)
                {
                    if (v == IMAGE.HERE)
                        ShowState(listViewStatus.Items[(int)n].Text + " started");
                    else if (v == IMAGE.PASS)
                        ShowState(listViewStatus.Items[(int)n].Text + " PASSED");
                    else if (v == IMAGE.FAIL)
                        ShowStateErr(listViewStatus.Items[(int)n].Text + " FAILED");
                    else if (v == IMAGE.RETRY)
                        ShowState(listViewStatus.Items[(int)n].Text + " (RETRY)");
                }
                listViewStatus.Items[(int)n].StateImageIndex = (int)v;
                listViewStatus.EnsureVisible((int)n);
            }
        }

        delegate void ShowSerialCallback(string sn);

        void ShowSerial(string sn)
        {
            if (this.textBoxSerial.InvokeRequired)
            {
                ShowSerialCallback d = new ShowSerialCallback(this.ShowSerial);
                this.BeginInvoke(d, sn);
            }
            else
            {
                textBoxSerial.Text = sn;
            }
        }

        private void serialPort1_PinChanged(object sender, System.IO.Ports.SerialPinChangedEventArgs e)
        {
            if (serialPort1.CtsHolding)
            {
                ItemState(ITEM.POWERON, IMAGE.FAIL);
                ShowStateErr("Overcurrent detected");
                SetState(STATE.POWEROFF);
            }
        }

        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            char[] buf = new char[10];
            int l = 1;

            while (l > 0)
            {
                try
                {
                    l = serialPort1.Read(buf, 0, buf.Length);
                }
                catch (Exception)
                {
                    l = 0;
                }

                for (int i = 0; i < l; i++)
                {
                    if (buf[i] == '\r' || buf[i] == '\n')
                    {
                        string x = rcv.Trim();
                        rcv= "";
                        if (x.Length > 0)
                        {
                            if (state == STATE.CHECKRAM && x.Length >= 8 && x.Contains("memory"))
                            {
                                ReplaceLog(x.Substring(0, 8), x);
                            }
                            else AddLog(x);
                            Process(x);
                        }
                    }
                    else
                    {
                        rcv += buf[i];
                        if (state == STATE.CHECKRAM && rcv.Length >= 8 && rcv.Contains("memory"))
                        {
                            ReplaceLog(rcv.Substring(0, 8), rcv);
                        }
                        //else AddLog("[" + rcv + "]");
                    }
                }
            }
        }

        private void PrintLabel(string sn)
        {            
            string labelfile = "Resources\\Holiday PCB 1.lbx";

            bpac.DocumentClass doc = null;
            object[] printers = null;
            
            try {
                doc = new bpac.DocumentClass();
            } catch (Exception) {}

            if (doc!=null)
                printers = (object[])doc.Printer.GetInstalledPrinters();

            if (doc==null || printers==null)
            {
                ShowStateErr("No Brother label printer found");
                ItemState(ITEM.PRINTLABEL, IMAGE.WARN);
                return;
            }
            
            //MessageBox.Show(printers.Length.ToString());

            ShowState("Printing label");

            if (!doc.Printer.IsPrinterOnline(printers[0].ToString()))
            {
                ShowStateErr("Printer " + printers[0].ToString() + " is offline");
                ItemState(ITEM.PRINTLABEL, IMAGE.WARN);
                return;
            }

            if (doc.Open(labelfile) != false)
            {
                doc.GetObject("objSerial").Text = sn; // "1308000123";
                //doc.GetObject("objSerial2").Text = sn; //"1308000123";
                //doc.GetObject("objMAC").Text = "12-34-56-78-90-AB";

                // uncomment next line to use media in printer, else we use media defined in label
                // doc.SetMediaById(doc.Printer.GetMediaId(), true);
                doc.StartPrint("", bpac.PrintOptionConstants.bpoDefault);
                doc.PrintOut(1, bpac.PrintOptionConstants.bpoDefault);
                doc.EndPrint();
                doc.Close();
                ItemState(ITEM.PRINTLABEL, IMAGE.PASS);
            }
            else
            {
                MessageBox.Show("Error opening " + labelfile + " (error code " + doc.ErrorCode + ")");
                ItemState(ITEM.PRINTLABEL, IMAGE.FAIL);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (endtest)
            {
                endtest = false;
                EndTest();
            }
            if (timeout > 0)
            {
                timeout -= timer1.Interval;
                if (timeout <= 0)
                {
                    timeout = 0;
                    Process("");
                }
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                try
                {
                    serialPort1.DtrEnable = false;
                    Thread.Sleep(1000);
                }
                catch (Exception) { }
            }
            EndTest();
        }

        void RunSBloader()
        {
            exitflag = 0;
            try
            {
                sbloader = new System.Diagnostics.Process();
                sbloader.StartInfo.WorkingDirectory = "Resources";
                //sbloader.StartInfo.FileName = "holitest.cmd";
                //sbloader.Start();

                //to use redirection must call exe directly and not use shell execute
                //also needs to avoid reading both stdout and stderr to to avoid blocking
                sbloader.StartInfo.FileName = "Resources\\sb_loader.exe";
                sbloader.StartInfo.Arguments = "-f holitest.sb";
                sbloader.StartInfo.UseShellExecute = false;
                sbloader.StartInfo.RedirectStandardOutput = true;
                //sbloader.StartInfo.RedirectStandardError = true;
                sbloader.StartInfo.CreateNoWindow = true;
                sbloader.Start();
            }
            catch (Exception ex)
            {
                ShowStateErr("sbloader: " + ex.Message);
                SetState(STATE.POWEROFF);
            }
        }

        void RunAVRdude()
        {
            exitflag = 0;
            try
            {
                avrdude = new System.Diagnostics.Process();

                // this seems most reliable method, and allows easy edit of cmd file for changes
                avrdude.StartInfo.WorkingDirectory = "Resources";
                avrdude.StartInfo.FileName = "holiprog.cmd";
                
                /*
                //to use redirection must call exe directly and not use shell execute
                //also needs to avoid reading both stdout and stderr to to avoid blocking
                //sometimes this method misbehaves!
                avrdude.StartInfo.FileName = "Resources\\avrdude.exe";
                avrdude.StartInfo.Arguments = "-q -pm328 -Pusb -cavrispmkII -u -Uhfuse:w:0xDE:m -Ulfuse:w:0xFF:m -Uefuse:w:0x04:m -Uflash:w:HolidayDuino04_plus_optiboot_holiday328_20MHz.hex:i";
                avrdude.StartInfo.UseShellExecute = false;
                //avrdude.StartInfo.RedirectStandardOutput = true;
                avrdude.StartInfo.RedirectStandardError = true;
                avrdude.StartInfo.CreateNoWindow = true;
                */

                avrdude.Start();

            }
            catch (Exception ex)
            {
                ShowStateErr("avrdude: " + ex.Message);
                SetState(STATE.POWEROFF);
            }
        }

        void KillProcesses()
        {
            //even if not null, it is possible these have not been fully initialised/started
            try
            {
                if (sbloader != null && !sbloader.HasExited) sbloader.Kill();
            }
            catch (Exception) { }
            
            try
            {
                if (avrdude != null && !avrdude.HasExited) avrdude.Kill();
            }
            catch (Exception) { }
         
            sbloader = null;
            avrdude = null;
        }

        void serialPortWrite(string s)
        {
            // just a wrapper to trap exceptions
            try
            {
                serialPort1.Write(s);
            }
            catch (Exception) { }
        }

        uint GetNextSerial()
        {
            if (snprefix == 0) return 0;
            
            uint low, high;
            if (snprefix > 9999)
            {
                low = snprefix * 100000;
                high = snprefix * 100000 + 99999;
            }
            else
            {
                low = snprefix * 1000000;
                high = snprefix * 1000000 + 999999;
            }

            try
            {
                SQLiteDatabase db = new SQLiteDatabase(defaultdb);

                string sql = "select id from serial where id >= " + low.ToString() + " and id <= " + high.ToString() + " order by id desc limit 1";
                DataTable d = db.GetDataTable(sql);

                if (d.Rows.Count > 0)
                    low = Convert.ToUInt32(d.Rows[0]["id"]);

                d.Dispose();
                db.Dispose();

                if (low + 1 > high)
                    return 0;

                return low + 1;
            }
            catch (Exception ex)
            {
                AddLogErr(ex.Message);
                return 0;
            }
        }

        bool AllocateSerial(uint n)
        {
            if (serialnum == 0) return false;

            try
            {
                SQLiteDatabase db = new SQLiteDatabase(defaultdb);
                DateTime dt = DateTime.Now;
                String adate = String.Format("{0:D4}/{1:D2}/{2:D2}", dt.Year, dt.Month, dt.Day);
                String atime = String.Format("{0:D2}:{1:D2}:{2:D2}", dt.Hour, dt.Minute, dt.Second);

                Dictionary<String, String> data = new Dictionary<string, string>();
                data.Add("id", n.ToString());
                data.Add("adate", adate);    // YYYY/MM/DD
                data.Add("atime", atime);
                data.Add("status", "");
                data.Add("mac", "");
                long row = db.Insert("serial", data);
                db.Dispose();
            }
            catch (Exception ex)
            {
                AddLogErr(ex.Message);
                return false;
            }
            return true;
        }

        void Process(string s)
        {
            switch (state)
            {
                case STATE.IDLE:
                    break;
                case STATE.BEGIN:
                    try
                    {
                        serialPort1.PortName = comboBoxPort.Text;
                        serialPort1.DtrEnable = false;
                        serialPort1.RtsEnable = false;
                        serialPort1.Open();

                        serialPort1.DtrEnable = true;
                        ItemState(ITEM.POWERON, IMAGE.HERE);

                        SetState(STATE.POWERON, 1000);
                    }
                    catch (Exception ex)
                    {
                        ShowStateErr("Serial port: " + ex.Message);
                        endtest = true;
                    }
                    break;
                case STATE.POWERON:
                    if (s.Contains("0x80"))
                    {
                        ItemState(ITEM.POWERON, IMAGE.PASS);
                        ItemState(ITEM.CHECKCOM, IMAGE.PASS);

                        serialPort1.RtsEnable = true;

                        SetState(STATE.BOOTLOAD, 1000);
                        ItemState(ITEM.BOOTLOAD, IMAGE.HERE);
                        RunSBloader();
                        break;
                    }

                    if (timeout == 0)
                    {
                        ShowStateErr("Initial communications error");
                        SetState(STATE.POWEROFF);
                    }
                    break;
                case STATE.BOOTLOAD:
                    if (sbloader != null)
                    {
                        try
                        {
                            while (!sbloader.StandardOutput.EndOfStream)
                            {
                                string x = sbloader.StandardOutput.ReadLine();
                                AddLog("sbloader stdout: " + x);
                            }
                        }
                        catch (Exception) { }
                        /*
                        try {
                            while (!sbloader.StandardError.EndOfStream)
                            {
                                string x = sbloader.StandardError.ReadLine();
                                AddLog("sbloader stderr: " + x);
                            }
                        }
                        catch (Exception) { }
                        */
                        try
                        {
                            if (sbloader.HasExited && exitflag == 0)
                            {
                                exitflag = 1;
                                // typical exit code seems to be 258??
                                AddLog("sbloader exit " + sbloader.ExitCode.ToString());
                            }
                        }
                        catch (Exception) { }
                    }
                    if (s.Contains("HolidayTest"))
                    {
                        ItemState(ITEM.BOOTLOAD, IMAGE.PASS);
                        SetState(STATE.CHECKOTP, 500);
                        ItemState(ITEM.CHECKOTP, IMAGE.HERE);
                        otpupdate = false;
                        serialPortWrite("o");
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    // allow extra retries to allow device driver install if needed
                    if (retry > 300)
                    {
                        ItemState(ITEM.BOOTLOAD, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    try
                    {
                        // sbloaded can exit before we see HolidayTest message, so don't restart too soon
                        if (sbloader.HasExited && ++exitflag == 10)
                        {
                            ItemState(ITEM.BOOTLOAD, IMAGE.RETRY);
                            RunSBloader();
                        }
                    }
                    catch (Exception) { }
                    timeout = 100;
                    break;
                case STATE.CHECKOTP:
                    // we can get here after sending an "o" for OTP read (otpupdate = false), or a "w!" when writing (otupdate = true)
                    if (s.Contains("ROM0=0x") && s.Contains("CUST0=0x") && s.Contains("CUST1=0x") && s.Contains("LOCK=0x"))
                    {
                        if (s.Contains("ROM0=0x00200008") && s.Contains("CUST0=0x4D434831"))   // boot setting & "MCH1"
                        {
                            if (serialnum == 0 || serialhex == "")
                            {
                                int pos = s.IndexOf("CUST1=0x");
                                serialhex = s.Substring(pos + 8, 8);
                                if (!uint.TryParse(serialhex, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out serialnum) ||
                                    serialnum == 0)
                                {
                                    ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                                    SetState(STATE.POWEROFF);
                                    break;
                                }
                                serialhex = serialnum.ToString("X8");
                                ShowSerial(serialnum.ToString());
                                AddLog("existing serial number " + serialnum.ToString("D10"));// + " = 0x" + serialhex);
                            }
                            if (!s.Contains("CUST1=0x" + serialhex))
                            {
                                    ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                                    SetState(STATE.POWEROFF);
                                    break;
                            }
                            
                            //check lock bits?  not so critical, and would need to only check last 2 bits - others could vary

                            // all good
                            ItemState(ITEM.CHECKOTP, IMAGE.PASS);
                            if (otpupdate) ItemState(ITEM.PROGOTP, IMAGE.PASS);

                            if (printonly)
                            {
                                SetState(STATE.POWEROFF);
                                ItemState(ITEM.PRINTLABEL, IMAGE.HERE);
                                PrintLabel(serialnum.ToString("D10"));
                                break;
                            }

                            SetState(STATE.CHECKRAM, 1000);
                            ItemState(ITEM.CHECKRAM, IMAGE.HERE);
                            serialPortWrite("t");
                            break;
                        }
                        if (printonly)
                        {
                            ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                            ShowStateErr("OTP not programmed - run normal tests");
                            SetState(STATE.POWEROFF);
                            break;
                        }
                        otpupdate = false;
                        if (!s.Contains("ROM0=0x00200008")) // boot setting
                        {
                            serialPortWrite("p00200008");
                            otpupdate = true;
                        }
                        if (s.Contains("CUST0=0x00000000"))
                        {
                            if (!otpupdate)
                            {
                                serialPortWrite("m4D434831");   // "MCH1"
                                otpupdate = true;
                            }
                        }
                        if (s.Contains("CUST1=0x00000000"))
                        {
                            serialnum = GetNextSerial();
                            if (serialnum==0)
                            {
                                ShowSerial("");
                                ShowStateErr("Could not allocate next serial number");
                                if (allowserialskip)
                                {
                                    AddLogErr("Continuing with rest of test without programming OTP!");
                                    timer1.Start();
                                    ItemState(ITEM.CHECKOTP, IMAGE.WARN);
                                    ItemState(ITEM.PROGOTP, IMAGE.WARN);
                                    SetState(STATE.CHECKRAM, 1000);
                                    ItemState(ITEM.CHECKRAM, IMAGE.HERE);
                                    serialPortWrite("t");
                                    break;
                                }
                                ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                                ShowStateErr("Could not allocate next serial number");
                                SetState(STATE.POWEROFF);
                                break;
                            }
                            serialhex = serialnum.ToString("X8");
                            ShowSerial(serialnum.ToString());
                            AllocateSerial(serialnum);
                            AddLog("allocated serial number " + serialnum.ToString("D10") + " = 0x" + serialhex);
                            ItemState(ITEM.PRINTLABEL, IMAGE.HERE);
                            PrintLabel(serialnum.ToString("D10"));
                            if (!otpupdate)
                            {
                                serialPortWrite("n" + serialhex);
                                otpupdate = true;
                            }
                        }
                        if (otpupdate)
                        {
                            ItemState(ITEM.PROGOTP, IMAGE.HERE);
                            SetState(STATE.PROGOTP, 200);
                            //serialPortWrite("l00000003");   // lock bits
                            break;
                        }
                        
                        // no changes needed
                        ItemState(ITEM.CHECKOTP, IMAGE.PASS);
                        SetState(STATE.CHECKRAM, 1000);
                        ItemState(ITEM.CHECKRAM, IMAGE.HERE);
                        serialPortWrite("t");
                        break;
                    }
                    if (s.Contains("error") || s.Contains("Abort"))
                    {
                        ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 3)
                    {
                        ItemState(ITEM.CHECKOTP, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    ItemState(ITEM.CHECKOTP, IMAGE.RETRY);
                    serialPortWrite("o");
                    timeout = 500;
                    break;
                case STATE.PROGOTP:
                    //verify values echoed back from p/m/n/l commands (in that order, although some could be skipped)
                    //when we see the l command (last one) we can initiate write, and go back to check response
                    if (s.Contains("=0x"))
                    {
                        if (s.StartsWith("p"))
                        {
                            if (s.Equals("p00200008=0x00200008"))
                            {
                                serialPortWrite("m4D434831");
                                SetState(state, 100);
                                break;
                            }
                            if (++retry <= 3)
                            {
                                serialPortWrite("p00200008");
                                timeout = 200;
                                break;
                            }
                        }
                        if (s.StartsWith("m"))
                        {
                            if (s.Equals("m4D434831=0x4D434831"))
                            {
                                serialPortWrite("n" + serialhex);
                                SetState(state, 100);
                                break;
                            }
                            if (++retry <= 3)
                            {
                                serialPortWrite("m4D434831");
                                timeout = 200;
                                break;
                            }
                        }
                        if (s.StartsWith("n"))
                        {
                            if (s.Equals("n" + serialhex + "=0x" + serialhex))
                            {
                                serialPortWrite("l00000003");
                                SetState(state, 100);
                                break;
                            }
                            if (++retry <= 3)
                            {
                                serialPortWrite("n" + serialhex);
                                timeout = 200;
                                break;
                            }
                        }
                        if (s.StartsWith("l"))
                        {
                            if (s.Equals("l00000003=0x00000003"))
                            {
                                serialPortWrite("w!");
                                // play it safe - just fake a write... by reading back...
                                //serialPortWrite("o");
                                SetState(STATE.CHECKOTP, 2000); // write OTP is a bit slow...
                                break;
                            }
                            if (++retry <= 3)
                            {
                                serialPortWrite("l00000003");
                                timeout = 200;
                                break;
                            }
                        }
                    }
                    if (s.Contains("error") || s.Contains("Abort"))
                    {
                        ItemState(ITEM.PROGOTP, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout>0) break;
                    retry++;
                    if (retry > 3)
                    {
                        ItemState(ITEM.PROGOTP, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    timeout = 100;
                    ItemState(ITEM.CHECKOTP, IMAGE.RETRY);
                    break;
                case STATE.CHECKRAM:
                    if (s.Contains("passed"))
                    {
                        ItemState(ITEM.CHECKRAM, IMAGE.PASS);
                        AddLog("memory test passed @" + retry.ToString());
                        SetState(STATE.AVRDUDE, 1000);
                        ItemState(ITEM.AVRDUDE, IMAGE.HERE);
                        RunAVRdude();
                        break;
                    }
                    if (s.Contains("error") || s.Contains("Abort"))
                    {
                        ItemState(ITEM.CHECKRAM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    // full ram test takes about 45 seconds, but trimmed that down by just testing every Nth byte
                    if (retry > 100)
                    {
                        ItemState(ITEM.CHECKRAM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    timeout = 100;
                    break;
                case STATE.AVRDUDE:
                    /*
                    try
                    {
                        if (avrdude == null || avrdude.HasExited)
                        {
                            SetState(STATE.CHECKATMEGA, 1000);
                            ItemState(ITEM.CHECKATMEGA, IMAGE.HERE);
                            serialPortWrite("?");
                        }
                    }
                    catch (Exception)
                    {
                        SetState(STATE.CHECKATMEGA, 1000);
                        ItemState(ITEM.CHECKATMEGA, IMAGE.HERE);
                        serialPortWrite("?");
                    }
                    */
                    if (avrdude != null)
                    {
                        /*
                        try
                        {
                            while (!avrdude.StandardOutput.EndOfStream)
                            {
                                string x = avrdude.StandardOutput.ReadLine();
                                AddLog("avrdude stdout: " + x);
                            }
                        }
                        catch (Exception) { }
                        */
                        try
                        {
                            while (!avrdude.StandardError.EndOfStream)
                            {
                                string x = avrdude.StandardError.ReadLine();
                                AddLog("avrdude stderr: " + x);
                            }
                        }
                        catch (Exception) { }
                        try
                        {
                            if (avrdude.HasExited && exitflag == 0)
                            {
                                exitflag = 1;
                                AddLog("avrdude exit " + avrdude.ExitCode.ToString() + " @" + retry.ToString());

                                if (avrdude.ExitCode == 0)
                                {
                                    ItemState(ITEM.AVRDUDE, IMAGE.PASS);
                                    SetState(STATE.CHECKATMEGA, 1000);
                                    ItemState(ITEM.CHECKATMEGA, IMAGE.HERE);
                                    serialPortWrite("?");
                                    break;
                                }
                                if (retry < 200)    // should be <1 if avrdude not called via shellexecute
                                {
                                    AddLogErr("avrdude error - retrying");
                                    retry++;
                                    timeout = 1000;
                                    ItemState(ITEM.AVRDUDE, IMAGE.RETRY);
                                    RunAVRdude();
                                    break;
                                }
                                ItemState(ITEM.AVRDUDE, IMAGE.FAIL);
                                // could try CHECKATMEGA anyway...
                                SetState(STATE.POWEROFF);
                                break;

                            }
                        }
                        catch (Exception) { }
                    }
                    /*
                    if (s.Contains("HolidayDuino"))
                    {
                        ItemState(ITEM.AVRDUDE, IMAGE.PASS);
                        ItemState(ITEM.CHECKATMEGA, IMAGE.PASS);
                        SetState(STATE.CHECKVOLTAGE, 200);
                        ItemState(ITEM.CHECKVOLTAGE, IMAGE.HERE);
                        serialPortWrite("V");
                        break;
                    }
                    */
                    if (timeout > 0) break;

                    retry++;
                    // avrdude takes about 18 seconds normally, so allow 50 seconds with retry (500x100ms)
                    // if avrdude isn't run via shellexecute, the timeouts never fire (stderr redirection blocks the app)
                    if (retry > 500)
                    {
                        ItemState(ITEM.AVRDUDE, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    timeout = 100;
                    break;
                case STATE.CHECKATMEGA:
                    if (s.Contains("HolidayDuino"))
                    {
                        //ItemState(ITEM.AVRDUDE, IMAGE.PASS);
                        ItemState(ITEM.CHECKATMEGA, IMAGE.PASS);
                        SetState(STATE.CHECKVOLTAGE, 200);
                        ItemState(ITEM.CHECKVOLTAGE, IMAGE.HERE);
                        serialPortWrite("V");
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        //ItemState(ITEM.AVRDUDE, IMAGE.FAIL);
                        ItemState(ITEM.CHECKATMEGA, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("?");
                    timeout = 100;
                    break;
                case STATE.CHECKVOLTAGE:
                    if (s.StartsWith("V4") || s.StartsWith("V5"))
                    {
                        ItemState(ITEM.CHECKVOLTAGE, IMAGE.PASS);
                        SetState(STATE.CHECKBUTTONS, 200);
                        ItemState(ITEM.CHECKBUTTONS, IMAGE.HERE);
                        serialPortWrite("B");
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.CHECKVOLTAGE, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("V");
                    timeout = 200;
                    break;
                case STATE.CHECKBUTTONS:
                    if (s.Equals("B0"))
                    {
                        ItemState(ITEM.CHECKBUTTONS, IMAGE.PASS);
                        SetState(STATE.CHECKANALOG, 200);
                        ItemState(ITEM.CHECKANALOG, IMAGE.HERE);
                        subitem = 0;
                        serialPortWrite(subitem.ToString() + "A");
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.CHECKBUTTONS, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("B");
                    timeout = 200;
                    break;
                case STATE.CHECKANALOG:
                    if (s.StartsWith(subitem.ToString() + "A"))
                    {
                        int val = 0;
                        if (!int.TryParse(s.Substring(2), out val) && ++retry < 5)
                        {
                            serialPortWrite(subitem.ToString() + "A");
                            timeout = 200;
                            break;
                        }
                        val -= (subitem+1)*205;
                        if (val<-5 || val>5)
                        {
                            AddLogErr("analog reading out of expected range - ignoring");
                        }
                        if (++subitem < 4)
                        {
                            serialPortWrite(subitem.ToString() + "A");
                            timeout = 200;
                            break;
                        }
                        ItemState(ITEM.CHECKANALOG, IMAGE.PASS);
                        SetState(STATE.ERASEEEPROM, 2000);
                        ItemState(ITEM.ERASEEEPROM, IMAGE.HERE);
                        serialPortWrite("i");
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.CHECKANALOG, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite(subitem.ToString() + "A");
                    timeout = 200;
                    break;
                case STATE.ERASEEEPROM:
                    if (s.Contains("erased"))
                    {
                        ItemState(ITEM.ERASEEEPROM, IMAGE.PASS);
                        SetState(STATE.PREPEEPROM, 200);
                        ItemState(ITEM.PREPEEPROM, IMAGE.HERE);
                        serialPortWrite("h");
                        break;
                    }
                    if (s.Contains("timeout"))
                    {
                        ItemState(ITEM.ERASEEEPROM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.ERASEEEPROM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("i");
                    timeout = 2000;
                    break;
                case STATE.PREPEEPROM:
                    if (s.Contains("prepped"))
                    {
                        ItemState(ITEM.PREPEEPROM, IMAGE.PASS);
                        SetState(STATE.CHECKEEPROM, 200);
                        ItemState(ITEM.CHECKEEPROM, IMAGE.HERE);
                        subitem = 0;
                        serialPortWrite(subitem.ToString() + "C");
                        break;
                    }
                    if (s.Contains("timeout"))
                    {
                        ItemState(ITEM.PREPEEPROM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.PREPEEPROM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("h");
                    timeout = 2000;
                    break;
                case STATE.CHECKEEPROM:
                    int [] eeval = { 104, 1, 8, 50, 0, 0 };
                    if (s.StartsWith(subitem.ToString() + "C"))
                    {
                        int val = 0;
                        if (!int.TryParse(s.Substring(2), out val) && ++retry < 5)
                        {
                            serialPortWrite(subitem.ToString() + "C");
                            timeout = 200;
                            break;
                        }
                        if (val!=eeval[subitem])
                        {
                            ItemState(ITEM.CHECKEEPROM, IMAGE.FAIL);
                            SetState(STATE.POWEROFF);
                            break;
                        }
                        if (++subitem < eeval.Length)
                        {
                            serialPortWrite(subitem.ToString() + "C");
                            timeout = 200;
                            break;
                        }
                        ItemState(ITEM.CHECKEEPROM, IMAGE.PASS);
                        SetState(STATE.CHECKSPI, 2000);
                        ItemState(ITEM.CHECKSPI, IMAGE.HERE);
                        serialPortWrite("1T");  // enable HolidayDuino test mode, but don't bother to validate...
                        serialPortWrite("s00" + testcolour);   // test SPI packet
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.CHECKEEPROM, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite(subitem.ToString() + "C");
                    timeout = 200;
                    break;
                case STATE.CHECKSPI:
                    if (s.Contains(":" + testcolour.Substring(0, 2) + "," + testcolour.Substring(2, 2) + "," + testcolour.Substring(4, 2)))
                    {
                        ItemState(ITEM.CHECKSPI, IMAGE.PASS);
                        SetState(STATE.ALLGOOD, 500);   // leave the test colour for a short while
                        break;
                    }
                    if (s.Contains("GPIO") || s.Contains("timeout"))
                    {
                        ItemState(ITEM.CHECKSPI, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    if (timeout > 0) break;
                    retry++;
                    if (retry > 5)
                    {
                        ItemState(ITEM.CHECKSPI, IMAGE.FAIL);
                        SetState(STATE.POWEROFF);
                        break;
                    }
                    serialPortWrite("s00" + testcolour);
                    timeout = 2000;
                    break;
                case STATE.ALLGOOD:
                    if (timeout > 0) break;
                    ShowStateGood("PASSED ALL TESTS");
                    if (serialnum == 0)
                        ShowStateErr("Passed tests but no serial number allocated");
                    serialPortWrite("s0000ff00");   // green
                    // keep green LED on for a little while
                    SetState(STATE.POWEROFF, 2500);
                    break;
                case STATE.POWEROFF:
                    if (timeout > 0) break;
                    try
                    {
                        serialPort1.RtsEnable = false;
                        serialPort1.DtrEnable = false;
                    }
                    catch (Exception) { }
                    SetState(STATE.END);
                    break;
                case STATE.END:
                    if (timeout==0)
                        endtest = true;
                    break;
            }
        }

        private void listViewLog_Resize(object sender, EventArgs e)
        {
            listViewLog.Columns[0].Width = listViewLog.Width - 24;
        }

        private void listViewStatus_Resize(object sender, EventArgs e)
        {
            listViewStatus.Columns[0].Width = listViewStatus.Width - 24;
        }

        private void comboBoxPort_DropDown(object sender, EventArgs e)
        {
            BuildPortList(comboBoxPort);
        }

        private void buttonNewBatch_Click(object sender, EventArgs e)
        {
            DateTime dt = DateTime.Now;
            snprefix = (uint)((dt.Year % 100) * 1000 + dt.Month * 10 + stationnum);
            AddLog("New SN prefix = " + snprefix.ToString());
            buttonGetNext_Click(null, null);
        }

        private void buttonGetNext_Click(object sender, EventArgs e)
        {
            serialnum = GetNextSerial();
            if (snprefix>0)
                AddLog("Next SN = " + serialnum.ToString());
        }

        private void buttonAllocate_Click(object sender, EventArgs e)
        {
            if (AllocateSerial(serialnum))
                AddLog("Allocated");
            else
                AddLogErr("Failed to allocate");
        }

        private void buttonCopyText_Click(object sender, EventArgs e)
        {
            if (listViewLog.Items.Count == 0)
            {
                MessageBox.Show("No text to copy");
                return;
            }
            StringBuilder sb = new StringBuilder();
            for (int i=0; i<listViewLog.Items.Count; i++)
                sb.AppendLine(listViewLog.Items[i].Text);
            Clipboard.SetText(sb.ToString(), TextDataFormat.Text);
            MessageBox.Show("Copied to clipboard");
        }

        private void comboBoxStationNum_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (loading) return;
            stationnum = int.Parse(comboBoxStationNum.Text);
            snprefix = 0;
            AddLog("Test Station # = " + stationnum.ToString());
            Registry.SetValue(keyname, "TestStationNum", stationnum);
        }
    }
}
