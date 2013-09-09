using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace HolidayTest1
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();

            String version = AssemblyVersion;

            richTextBox1.Rtf = 
"{\\rtf1\\ansi\\ansicpg1252\\deff0\\deflang3081\\deflangfe3081{\\fonttbl{\\f0\\froman\\fprq2\\fcharset0 Times New Roman;}{\\f1\\fswiss\\fprq2\\fcharset0 Calibri;}}\r\n" +
"{\\colortbl ;\\red0\\green0\\blue255;}\r\n" +
"{\\*\\generator Msftedit 5.41.21.2510;}\\viewkind4\\uc1\\pard\\qc\\lang9\\f0\\fs22\\par\r\n" +
"\\b\\f1 MooresCloud Holiday Test Program\\par\r\n" +
"\\b0 Version " + version + " \\par\r\n" +
"\\par\r\n" +
"Developed for MooresCloud\\par\r\n" +
"by Kean Electronics\\par\r\n" +
"{\\field{\\*\\fldinst{HYPERLINK \"www.kean.com.au\"}}{\\fldrslt{\\ul\\cf1 www.kean.com.au}}}\\f0\\fs22\\par\r\n" +
"\\par\r\n" +
"\\f1 (c) MooresCloud Pty Ltd 2013\\f0\\par\r\n" +
"}";
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion
    }
}
