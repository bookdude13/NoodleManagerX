﻿using System;
using Newtonsoft.Json;
using DynamicData;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.IO.Compression;
using System.Runtime.Serialization;

namespace NoodleManagerX.Models
{
    class MapHandler : GenericHandler
    {
        public override ItemType itemType { get; set; } = ItemType.Map;

        public override string searchQuery { get; set; } = "{\"$or\":[{\"title\":{\"$contL\":\"<value>\"}},{\"artist\":{\"$contL\":\"<value>\"}},{\"mapper\":{\"$contL\":\"<value>\"}}]}";
        public override string apiEndpoint { get; set; } = "https://synthriderz.com/api/beatmaps";

        public override void LoadLocalItems()
        {
            string directory = Path.Combine(MainViewModel.s_instance.settings.synthDirectory, "CustomSongs");
            if (Directory.Exists(directory))
            {
                Task.Run(async () =>
                {
                    List<LocalItem> tmp = new List<LocalItem>();
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        if (Path.GetExtension(file) == ".synth")
                        {
                            try
                            {
                                using (ZipArchive archive = ZipFile.OpenRead(file))
                                {
                                    foreach (ZipArchiveEntry entry in archive.Entries)
                                    {
                                        if (entry.FullName == "synthriderz.meta.json")
                                        {
                                            using (StreamReader sr = new StreamReader(entry.Open()))
                                            {
                                                LocalItem localItem = JsonConvert.DeserializeObject<LocalItem>(await sr.ReadToEndAsync());
                                                localItem.itemType = ItemType.Map;
                                                tmp.Add(localItem);
                                            }
                                        }
                                    }
                                }
                            }
                            catch
                            {
                                Console.WriteLine("Deleting corrupted file " + Path.GetFileName(file));
                                File.Delete(file);
                            }
                        }
                    }
                    MainViewModel.s_instance.localItems.Add(tmp);
                });
            }
        }

        public override dynamic DeserializePage(string json)
        {
            return JsonConvert.DeserializeObject<MapPage>(json);
        }

    }

    [DataContract]
    class MapItem : GenericItem
    {
        [DataMember] public string title { get; set; }
        [DataMember] public string artist { get; set; }
        [DataMember] public string mapper { get; set; }
        [DataMember] public string duration { get; set; }
        [DataMember] public string[] difficulties { get; set; }
        [DataMember] public string hash { get; set; }
        public override string target { get; set; } = "CustomSongs";
        public override ItemType itemType { get; set; } = ItemType.Map;
    }


    class MapPage : GenericPage
    {
        public List<MapItem> data;

        public override ItemType itemType { get; set; } = ItemType.Map;
    }
}
