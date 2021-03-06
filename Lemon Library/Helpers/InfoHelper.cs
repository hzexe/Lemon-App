﻿using System;
using System.Collections.Generic;
using System.IO;

namespace LemonLibrary
{
    public class InfoHelper {
        public enum NowPage {
            Search,Top,SingerItem,GDItem
        }
        ////////Music Helper////////
        public class Music
        {
            public string MusicName { set; get; } = "";
            public string MusicName_Lyric { get; set; } = "";
            public List<MusicSinger> Singer { set; get; } = new List<MusicSinger>();
            public string SingerText { get; set; } = "";
            public string MusicID { set; get; } = "";
            public string ImageUrl { set; get; } = "";
        }
        public class MusicSinger {
            public string Name { set; get; }
            public string Photo { set; get; }
            public string Mid { set; get; }
        }
        public class MusicGD
        {
            public string Name { set; get; }
            public string Photo { set; get; }
            public string ID { set; get; }
        }
        public class MusicTop
        {
            public string Name { set; get; }
            public string Photo { set; get; }
            public string ID { set; get; }
        }
        public class MusicPL {
            public string name { get; set; }
            public string img { get; set; }
            public string like { get; set; }
            public string text { get; set; }
            public string commentid { get; set; }
            public string time { get; set; }
            public bool ispraise { get; set; }
        }
        public class MusicRadioList {
            public List<MusicRadioListItem> Hot { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Evening{ set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Love { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Theme { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Changjing { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Style { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Lauch { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> People { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> MusicTools { set; get; } = new List<MusicRadioListItem>();
            public List<MusicRadioListItem> Diqu { set; get; } = new List<MusicRadioListItem>();
        }
        public class MusicRadioListItem {
            public string Name { set; get; }
            public string Photo { set; get; }
            public string ID { set; get; }
        }
        public class MusicFLGDIndexItemsList
        {
            public List<MusicFLGDIndexItems> Hot { get; set; } = new List<MusicFLGDIndexItems>();
            public List<MusicFLGDIndexItems> Lauch { get; set; } = new List<MusicFLGDIndexItems>();
            public List<MusicFLGDIndexItems> LiuPai { get; set; } = new List<MusicFLGDIndexItems>();
            public List<MusicFLGDIndexItems> Theme { get; set; } = new List<MusicFLGDIndexItems>();
            public List<MusicFLGDIndexItems> Heart { get; set; } = new List<MusicFLGDIndexItems>();
            public List<MusicFLGDIndexItems> Changjing { get; set; } = new List<MusicFLGDIndexItems>();
        }
        public class MusicFLGDIndexItems {
            public string name { get; set; }
            public string id { get; set; }
        }
        public class MusicGData {
            public List<Music> Data { get; set; } = new List<Music>();
            public string name { get; set; }
            public string pic { get; set; }
            public string id { get; set; }
            public bool IsOwn = false;
            public List<string> ids = new List<string>();
        }
        ////////Weather Helper/////
        public class Weather {
            public string Qiwen { get; set; }
            public string KongQiZhiLiang { get; set; }
            public string MiaoShu { get; set; }
            public string TiGan { get; set; }
            public string FenSu { get; set; }
            public string NenJianDu { get; set; }
            public List<WeatherByDay> Data { get; set; }

        }
        public class WeatherByDay {
            public string Date { set; get; }
            public string Icon { set; get; }
            public string QiWen { set; get; }
        }
    }
}
