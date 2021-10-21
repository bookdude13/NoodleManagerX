﻿using System;
using Newtonsoft.Json;
using DynamicData;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace NoodleManagerX.Models
{
    class PlaylistHandler : GenericHandler
    {
        public override ItemType itemType { get; set; } = ItemType.Playlist;
        public override string apiEndpoint { get; set; } = "https://synthriderz.com/api/playlists";
        public override string folder { get; set; } = "Playlist";
        public override string[] extensions { get; set; } = { ".playlist" };

        public override dynamic DeserializePage(string json)
        {
            return JsonConvert.DeserializeObject<PlaylistPage>(json);
        }
    }

    [DataContract]
    class PlaylistItem : GenericItem
    {
        [DataMember] public string name { get; set; }
        [DataMember] public User user { get; set; }
        [DataMember] public List<PlaylistEntryItem> items { get; set; }
        public string duration { get { return items != null ? items.Count().ToString() : "0"; } }
        public override string display_title
        {
            get { return name; }
        }
        public override string display_creator
        {
            get { return user.username; }
        }

        public override string target { get; set; } = "Playlist";
        public override ItemType itemType { get; set; } = ItemType.Playlist;
    }

    [DataContract]
    class PlaylistEntryItem
    {
        [DataMember] public int id { get; set; }
    }

    class PlaylistPage : GenericPage
    {
        public List<PlaylistItem> data;
    }
}

