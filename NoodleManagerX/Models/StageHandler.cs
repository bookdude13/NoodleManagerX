﻿using System;
using Newtonsoft.Json;
using DynamicData;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NoodleManagerX.Models
{
    class StageHandler : GenericHandler
    {
        public override ItemType itemType { get; set; } = ItemType.Stage;

        public override string allParameters { get; set; } = "\"name\":{\"$contL\":\"<value>\"}},{\"user.username\":{\"$contL\":\"<value>\"}";
        public override string select { get; set; } = "name,user";
        public override string apiEndpoint { get; set; } = "https://synthriderz.com/api/models/stages";

        public override void LoadLocalItems()
        {
            if (MainViewModel.s_instance.settings.synthDirectory != "")
            {
                string directory = Path.Combine(MainViewModel.s_instance.settings.synthDirectory, "CustomStages");
                if (Directory.Exists(directory))
                {
                    List<LocalItem> tmp = new List<LocalItem>();
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        if (Path.GetExtension(file) == ".stage" || Path.GetExtension(file) == ".spinstage")
                        {
                            tmp.Add(new LocalItem(-1, "", Path.GetFileName(file), File.GetLastWriteTime(file), ItemType.Stage));
                        }
                    }
                    MainViewModel.s_instance.localItems.AddRange(tmp);
                }
            }
        }

        public override dynamic DeserializePage(string json)
        {
            return JsonConvert.DeserializeObject<StagePage>(json);
        }

    }

    [DataContract]
    class StageItem : GenericItem
    {
        [DataMember] public string name { get; set; }
        [DataMember] public User user { get; set; }
        public override string display_title
        {
            get { return name; }
        }
        public override string display_creator
        {
            get { return user.username; }
        }
        public override string[] display_difficulties
        {
            get { return null; }
        }
        public override string target { get; set; } = "CustomStages";
        public override ItemType itemType { get; set; } = ItemType.Stage;
    }


    class StagePage : GenericPage
    {
        public List<StageItem> data;
    }
}

