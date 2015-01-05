using CloudPanel.Modules.Base.Interface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace CloudPanel.Modules.Base.Class
{
    public class MailboxDatabase : IDatabase
    {
        /// <summary>
        /// The path where the database is stored
        /// </summary>
        private string _edbfilepath;
        public string EDBFilePath
        {
            get { return _edbfilepath; }
            set { _edbfilepath = value; }
        }

        /// <summary>
        /// The path where the logs are stored
        /// </summary>
        private string _logfolderpath;
        public string LogFolderPath
        {
            get { return _logfolderpath; }
            set { _logfolderpath = value; }
        }

        /// <summary>
        /// The size of the database
        /// </summary>
        private string _databasesize;
        public string DatabaseSize
        {
            get
            {
                if (!string.IsNullOrEmpty(_databasesize))
                    return FormatExchangeSize(_databasesize);
                else
                    return _databasesize;
            }
            set { _databasesize = value; }
        }

        /// <summary>
        /// If it is a mailbox database or not
        /// </summary>
        private bool _ismailboxdatabase;
        public bool IsMailboxDatabase
        {
            get { return _ismailboxdatabase; }
            set { _ismailboxdatabase = value; }
        }

        /// <summary>
        /// If it is a public folder database or not
        /// </summary>
        private bool _ispublicfolderdatabase;
        public bool IsPublicFolderDatabase
        {
            get { return _ispublicfolderdatabase; }
            set { _ispublicfolderdatabase = value; }
        }

        /// <summary>
        /// Formats the exchange size from 434.00GB (23,23,34,234 bytes) to just kilobytes
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        private string FormatExchangeSize(string size)
        {
            if (string.IsNullOrEmpty(size))
            {
                return "0";
            }
            else
            {
                string newSize = size;

                string[] stringSeparators = new string[] { "TB (", "GB (", "MB (", "KB (", "B (" };

                if (newSize.Contains("TB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "TB");
                else if (newSize.Contains("GB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "GB");
                else if (newSize.Contains("MB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "MB");
                else if (newSize.Contains("KB ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "KB");
                else if (newSize.Contains("B ("))
                    newSize = ConvertToKB(decimal.Parse(newSize.Split(stringSeparators, StringSplitOptions.None)[0].Trim(), CultureInfo.InvariantCulture), "B");

                return newSize.Trim();
            }
        }

        /// <summary>
        /// Converts the size from TB,GB,MB to KB
        /// </summary>
        /// <param name="size"></param>
        /// <param name="sizeType"></param>
        /// <returns></returns>
        private string ConvertToKB(decimal size, string sizeType)
        {
            decimal newSize = 0;

            switch (sizeType)
            {
                case "TB":
                    newSize = size * 1024 * 1024 * 1024;
                    break;
                case "GB":
                    newSize = size * 1024 * 1024;
                    break;
                case "MB":
                    newSize = size * 1024;
                    break;
                default:
                    newSize = size;
                    break;
            }

            return newSize.ToString(CultureInfo.InvariantCulture);
        }
    }
}
