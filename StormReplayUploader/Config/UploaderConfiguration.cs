﻿using System;
using System.Configuration;
using System.IO;

namespace StormReplayUploader.Config
{
    public class UploaderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("replayDirectory", IsRequired = false)]
        public string ReplayDirectory
        {
            get
            {
                var replayDirectory = (string)this["replayDirectory"];

                return String.IsNullOrEmpty(replayDirectory) ? DefaultReplayDirectory : replayDirectory;
            }
            set
            {
                this["replayDirectory"] = value;
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
