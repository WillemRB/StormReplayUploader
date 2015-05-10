using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StormReplayUploader
{
    public class UploaderConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("replayDirectory", IsRequired = false)]
        public string ReplayDirectory
        {
            get
            {
                return (string)this["replayDirectory"];
            }
            set
            {
                this["replayDirectory"] = value;
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
}
