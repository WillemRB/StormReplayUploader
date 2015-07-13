using System;
using System.Configuration;
using System.IO;

namespace StormReplayUploader.Config
{
    public class UploaderConfiguration : ConfigurationSection
    {
        /// <summary>
        /// The directory that will be used to upload StormReplay files from.
        /// By default this directory is found in the Document and Settings folder
        /// of the user.
        /// </summary>
        [ConfigurationProperty("replayDirectory", IsRequired = false)]
        public string ReplayDirectory
        {
            get
            {
                var replayDirectory = (string)this["replayDirectory"];

                return String.IsNullOrEmpty(replayDirectory) ? DefaultReplayDirectory : replayDirectory;
            }
        }
        
        /// <summary>
        /// The interval between updates in seconds. 
        /// </summary>
        /// <remarks>
        /// The default interval is to run every 15 minutes.
        /// The minimum interval is 5 minutes.
        /// </remarks>
        [ConfigurationProperty("updateInterval", IsRequired = false)]
        public int UpdateInterval
        {
            get
            {
                var interval = (int)this["updateInterval"];

                return (interval == 0) ? 900 : Math.Max(interval, 300);
            }
        }

        [ConfigurationProperty("targets")]
        [ConfigurationCollection(typeof(TargetCollection), AddItemName = "target")]
        public TargetCollection Targets
        {
            get
            {
                return (TargetCollection)base["targets"];
            }
        }

        /// <summary>
        /// Returns the path to the replay files for the current user.
        /// But because the service runs using a special account this does not work.
        /// </summary>
        public string DefaultReplayDirectory
        {
            get
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                return Path.Combine(documents, "Heroes of the Storm", "Accounts");
            }
        }

        public string ReplayFilter
        {
            get
            {
                return "*.StormReplay";
            }
        }
    }

    public class TargetCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new TargetElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            var target = (TargetElement)element;
            return target.AssemblyName + "+" + target.TypeName;
        }
    }

    public class TargetElement : ConfigurationElement
    {
        [ConfigurationProperty("assemblyName")]
        public string AssemblyName
        {
            get
            {
                return (string)this["assemblyName"];
            }
            set
            {
                this["assemblyName"] = value;
            }
        }

        [ConfigurationProperty("typeName")]
        public string TypeName
        {
            get
            {
                return (string)this["typeName"];
            }
            set
            {
                this["typeName"] = value;
            }
        }
    }
}
