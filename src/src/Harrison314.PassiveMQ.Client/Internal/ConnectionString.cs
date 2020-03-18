using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client.Internal
{
    internal class ConnectionString : ICloneable
    {
        protected Dictionary<string, string> dictionary;

        public ConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.dictionary = new Dictionary<string, string>();
            this.ParseConnectionString(connectionString);
        }

        public ConnectionString()
        {
            this.dictionary = new Dictionary<string, string>();
        }


        private void ParseConnectionString(string connectionStr)
        {
            Regex regex = new Regex("(^|;)\\s*(?<name>[A-Za-z0-9_]+)\\s*=\\s*(?<value>([^;]|;;)*)", RegexOptions.IgnoreCase);
            foreach (Match math in regex.Matches(connectionStr))
            {
                string name = math.Groups["name"].Value.Trim();
                string value = math.Groups["value"].Value.Replace(";;", ";").Trim();
                if (string.IsNullOrEmpty(name))
                {
                    string message = string.Format("Name in position {0} is empty.", math.Index);
                    throw new ArgumentException(message);
                }

                if (string.IsNullOrEmpty(value))
                {
                    string message = string.Format("Value in position {0} is empty.", math.Index);
                    throw new ArgumentException(message);
                }

                this.dictionary.Add(name, value);
            }
        }


        public int GetInt(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            int result;
            if (!int.TryParse(value, out result))
            {
                string message = string.Format("Key '{0}' value is not int", name);
                throw new ArgumentException(message);
            }

            return result;
        }

        public bool TryGetInt(string name, out int value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string strValue = null;
            if (!this.dictionary.TryGetValue(name, out strValue))
            {
                value = default(int);
                return false;
            }

            if (!int.TryParse(strValue, out value))
            {
                string message = string.Format("Key '{0}' value is not int", name);
                throw new ArgumentException(message);
            }

            return true;
        }

        public long GetLong(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            long result;
            if (!long.TryParse(value, out result))
            {
                string message = string.Format("Key '{0}' value is not long.", name);
                throw new ArgumentException(message);
            }

            return result;
        }

        public bool TryGetLong(string name, out long value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string strvalue = null;
            if (!this.dictionary.TryGetValue(name, out strvalue))
            {
                value = default(long);
                return false;
            }

            if (!long.TryParse(strvalue, out value))
            {
                string message = string.Format("Key '{0}' value is not long.", name);
                throw new ArgumentException(message);
            }

            return true;
        }

        public double GetDouble(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            double result;
            if (!double.TryParse(value, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out result))
            {
                string message = string.Format("Key '{0}' value is not double", name);
                throw new ArgumentException(message);
            }

            return result;
        }


        public bool TryGetDouble(string name, out double value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string strValue = null;
            if (!this.dictionary.TryGetValue(name, out strValue))
            {
                value = default(double);
                return false;
            }


            if (!double.TryParse(strValue, System.Globalization.NumberStyles.Number | System.Globalization.NumberStyles.AllowDecimalPoint, System.Globalization.CultureInfo.InvariantCulture, out value))
            {
                string message = string.Format("Key '{0}' value is not double", name);
                throw new ArgumentException(message);
            }

            return true;
        }

        public Type GetType(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            Type type = Type.GetType(value.Trim());
            if (type == null)
            {
                string message = string.Format("Key '{0}' value is not Type.", name);
                throw new ArgumentException(message);
            }

            return type;
        }

        public bool TryGetType(string name, out Type value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string strValue = null;
            if (!this.dictionary.TryGetValue(name, out strValue))
            {
                value = null;
                return false;
            }

            value = Type.GetType(strValue.Trim());
            if (value == null)
            {
                string message = string.Format("Key '{0}' value is not Type", name);
                throw new ArgumentException(message);
            }

            return true;
        }

        public string GetString(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            return value;
        }

        public bool TryGetString(string name, out string value)
        {

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            value = null;
            return this.dictionary.TryGetValue(name, out value);
        }

        public Guid GetGuid(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string value = null;
            if (!this.dictionary.TryGetValue(name, out value))
            {
                string message = string.Format("Key '{0}' in connection string not found.", name);
                throw new ArgumentException(message);
            }

            Guid result;
            if (!Guid.TryParse(value, out result))
            {
                string message = string.Format("Key '{0}' value is not Guid.", name);
                throw new ArgumentException(message);
            }

            return result;
        }

        public bool TryGetGuid(string name, out Guid value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            string strValue = null;
            if (!this.dictionary.TryGetValue(name, out strValue))
            {
                value = Guid.Empty;
                return false;
            }

            if (!Guid.TryParse(strValue, out value))
            {
                string message = string.Format("Key '{0}' value is not Guid.", name);
                throw new ArgumentException(message);
            }

            return true;
        }


        public void Add(string name, string value)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            this.dictionary.Add(name, value);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in this.dictionary)
            {
                sb.Append(pair.Key);
                sb.Append('=');
                sb.Append(pair.Value.Replace(";", ";;"));
                sb.Append(';');
            }

            return sb.ToString();
        }

        public string ToString(bool escape)
        {
            if (escape)
            {
                return this.ToString();
            }

            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in this.dictionary)
            {
                sb.Append(pair.Key);
                sb.Append('=');
                sb.Append(pair.Value);
                sb.Append(';');
            }

            return sb.ToString();
        }

        public virtual object Clone()
        {
            ConnectionString connectionString = new ConnectionString();
            foreach (KeyValuePair<string, string> pair in this.dictionary)
            {
                connectionString.dictionary.Add(pair.Key, pair.Value);
            }

            return connectionString;
        }
    }
}
