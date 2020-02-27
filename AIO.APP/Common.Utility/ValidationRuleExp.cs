using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AIO.APP.Common.Utility
{
   public  class ValidationRuleExp:ValidationRule
    {
        public int checkType { get; set; }
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                if (checkType == 0)
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString().Trim()) || !IsNumeric(value.ToString().Trim()))
                    {
                        return new ValidationResult(false, "error numeric format");
                    }
                }
                else if (checkType == 1)
                {
                    if (value == null || string.IsNullOrEmpty(value.ToString().Trim()) || !IsIp(value.ToString().Trim()))
                    {
                        return new ValidationResult(false, "error Ip Format");
                    }
                }
                return new ValidationResult(true, null);
            }
            catch (Exception e)
            {
                return new ValidationResult(false, e.Message);
            }
        }
 
        private bool IsNumeric(string numericStr)
        {
            return NumericRegex.IsMatch(numericStr);
         
        }
 
        private bool IsIp(string s)
        {
            return IpRegex.IsMatch(s);
            
        }
        
        private static readonly Regex NumericRegex = new Regex(@"^[-]?[0-9]+(\.[0-9]+)?$");

        private static readonly Regex IpRegex = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");


    }
}
