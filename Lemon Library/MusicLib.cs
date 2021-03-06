﻿using LemonLibrary.Helpers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using static LemonLibrary.InfoHelper;
/*
   作者:Twilight./Lemon        QQ:2728578956
   请保留版权信息，侵权必究。
     
     Author:Twilight./Lemon QQ:2728578956
Please retain the copyright information, rights reserved.
     */

namespace LemonLibrary
{
    public class MusicLib
    {
        public MusicLib(LyricView LV, string id)
        {
            if (!Directory.Exists(Settings.USettings.DownloadPath))
                Directory.CreateDirectory(Settings.USettings.DownloadPath);
            if (!Directory.Exists(Settings.USettings.CachePath))
                Directory.CreateDirectory(Settings.USettings.CachePath);
            if (!Directory.Exists(Settings.USettings.CachePath + "Music\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Music\\");
            if (!Directory.Exists(Settings.USettings.CachePath + "Lyric\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Lyric\\");
            if (!Directory.Exists(Settings.USettings.CachePath + "Image\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Image\\");
            lv = LV;
            qq = id;
            GetMusicLikeGDid();
        }
        public MusicLib()
        {
            if (!Directory.Exists(Settings.USettings.DownloadPath))
                Directory.CreateDirectory(Settings.USettings.DownloadPath);
            if (!Directory.Exists(Settings.USettings.CachePath))
                Directory.CreateDirectory(Settings.USettings.CachePath);
            if (!Directory.Exists(Settings.USettings.CachePath + "Music\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Music\\");
            if (!Directory.Exists(Settings.USettings.CachePath + "Lyric\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Lyric\\");
            if (!Directory.Exists(Settings.USettings.CachePath + "Image\\"))
                Directory.CreateDirectory(Settings.USettings.CachePath + "Image\\");
        }
        public static MediaPlayer mp = new MediaPlayer();
        public LyricView lv;
        public static string qq = "";
        public static string MusicLikeGDid = "";
        public static string MusicLikeGDdirid = "";
        public async Task<List<Music>> GetSingerMusicByIdAsync(string mid,int osx=1) {
            int begin = (osx - 1) * 30;
            JObject o = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/v8/fcg-bin/fcg_v8_singer_track_cp.fcg?hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0&ct=24&singermid={mid}&order=listen&begin={begin}&num=30&songstatus=1"));
            List<Music> dt = new List<Music>();
            JToken dtl = o["data"]["list"];
            foreach (JToken dtli in dtl) {
                var dsli = dtli["musicData"];
                Music m = new Music();
                m.MusicName = dsli["songname"].ToString();
                m.MusicName_Lyric = dsli["albumdesc"].ToString();
                string Singer = "";
                List<MusicSinger> lm = new List<MusicSinger>();
                for (int osxc = 0; osxc != dsli["singer"].Count(); osxc++) {
                    Singer += dsli["singer"][osxc]["name"] + "&";
                    lm.Add(new MusicSinger() { Name = dsli["singer"][osxc]["name"].ToString(), Mid= dsli["singer"][osxc]["mid"].ToString() });
                }
                m.Singer = lm;
                m.SingerText = Singer.Substring(0, Singer.LastIndexOf("&"));
                m.MusicID = dsli["songmid"].ToString();
                var amid = dsli["albummid"].ToString();
                if (amid == "001ZaCQY2OxVMg")
                    m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T001R300x300M000{dsli["singer"][0]["mid"].ToString()}.jpg?max_age=2592000";
                else m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T002R300x300M000{amid}.jpg?max_age=2592000";
                dt.Add(m);
            }
            return dt;
        }
        public async Task<List<Music>> SearchMusicAsync(string Content, int osx = 1)
        {
            if (HttpHelper.IsNetworkTrue)
            {
                JObject o = JObject.Parse(await HttpHelper.GetWebAsync($"http://59.37.96.220/soso/fcgi-bin/client_search_cp?format=json&t=0&inCharset=GB2312&outCharset=utf-8&qqmusic_ver=1302&catZhida=0&p={osx}&n=20&w={HttpUtility.UrlDecode(Content)}&flag_qc=0&remoteplace=sizer.newclient.song&new_json=1&lossless=0&aggr=1&cr=1&sem=0&force_zonghe=0"));
                List<Music> dt = new List<Music>();
                int i = 0;
                var dsl = o["data"]["song"]["list"];
                while (i < dsl.Count())
                {
                    var dsli = dsl[i];
                    Music m = new Music();
                    m.MusicName = dsli["title"].ToString();
                    m.MusicName_Lyric = dsli["lyric"].ToString();
                    string Singer = "";
                    List<MusicSinger> lm = new List<MusicSinger>();
                    for (int osxc = 0; osxc != dsli["singer"].Count(); osxc++)
                    {
                        Singer += dsli["singer"][osxc]["name"] + "&";
                        lm.Add(new MusicSinger() { Name = dsli["singer"][osxc]["name"].ToString(), Mid = dsli["singer"][osxc]["mid"].ToString() });
                    }
                    m.Singer = lm;
                    m.SingerText = Singer.Substring(0, Singer.LastIndexOf("&"));
                    m.MusicID = dsli["mid"].ToString();
                    var amid = dsli["album"]["mid"].ToString();
                    if (amid == "001ZaCQY2OxVMg")
                        m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T001R300x300M000{dsli["singer"][0]["mid"].ToString()}.jpg?max_age=2592000";
                    else m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T002R300x300M000{amid}.jpg?max_age=2592000";
                    dt.Add(m);
                    i++;
                }
                return dt;
            }
            else return null;
        }
        public async Task<List<string>> Search_SmartBoxAsync(string key)
        {
            var data = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/splcloud/fcgi-bin/smartbox_new.fcg?key={HttpUtility.UrlDecode(key)}&utf8=1&is_xml=0&loginUin={Settings.USettings.LemonAreeunIts}&qqmusic_ver=1592&searchid=3DA3E73D151F48308932D9680A3A5A1722872&pcachetime=1535710304"))["data"];
            List<String> list = new List<String>();
            var song = data["song"]["itemlist"];
            for (int i = 0; i < song.Count(); i++)
            {
                var o = song[i];
                list.Add("歌曲:" + o["name"] + " - " + o["singer"]);
            }
            var album = data["album"]["itemlist"];
            for (int i = 0; i < album.Count(); i++)
            {
                var o = album[i];
                list.Add("专辑:" + o["singer"] + " - 《" + o["name"] + "》");
            }
            var singer = data["singer"]["itemlist"];
            for (int i = 0; i < singer.Count(); i++)
            {
                var o = singer[i];
                list.Add("歌手:" + o["singer"]);
            }
            return list;
        }
        public async void GetMusicLikeGDid()
        {
            string dta = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/rsc/fcgi-bin/fcg_get_profile_homepage.fcg?loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&cid=205360838&ct=20&userid={Settings.USettings.LemonAreeunIts}&reqfrom=1&reqtype=0");
            JObject o = JObject.Parse(dta);
            string id = "";
            foreach (var a in o["data"]["mymusic"]) {
                if (a["title"].ToString() == "我喜欢") {
                    id = a["id"].ToString();
                    break;
                }
            }
            MusicLikeGDid = id;
            Console.WriteLine("kkkkkkkkkkkkkkk" + MusicLikeGDid);
            MusicLikeGDdirid = await GetGDdiridByNameAsync("我喜欢");
        }
        public static async Task<MusicGData> GetGDAsync(string id = "2591355982", Action<Music, bool> callback = null, Window wx = null)
        {
            var s = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/qzone/fcg-bin/fcg_ucc_getcdinfo_byids_cp.fcg?type=1&json=1&utf8=1&onlysong=0&disstid={id}&format=json&g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0", Encoding.UTF8);
            JObject o = JObject.Parse(s);
            var dt = new MusicGData();
            var c0 = o["cdlist"][0];
            dt.name = c0["dissname"].ToString();
            dt.pic = c0["logo"].ToString();
            dt.id = id;
            dt.ids = c0["songids"].ToString().Split(',').ToList();
            dt.IsOwn = c0["login"].ToString() == c0["uin"].ToString();
            var c0s = c0["songlist"];
            foreach (var c0si in c0s)
            {
                string singer = "";
                var c0sis = c0si["singer"];
                List<MusicSinger> lm = new List<MusicSinger>();
                foreach (var cc in c0sis) {
                    singer += cc["name"].ToString() + "&";
                    lm.Add(new MusicSinger() { Name = cc["name"].ToString(),
                        Mid = cc["mid"].ToString()
                    });
                }
                Music m = new Music();
                try
                {
                    m.MusicName = c0si["songname"].ToString();
                    m.MusicName_Lyric = c0si["albumdesc"].ToString();
                    m.Singer = lm;
                    m.SingerText = singer.Substring(0, singer.Length - 1);
                    m.MusicID = c0si["songmid"].ToString();
                    var amid = c0si["albummid"].ToString();
                    if (amid == "001ZaCQY2OxVMg")
                        m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T001R300x300M000{c0si["singer"][0]["mid"].ToString()}.jpg?max_age=2592000";
                    else m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T002R300x300M000{amid}.jpg?max_age=2592000";
                }//莫名其妙的System.NullReferenceException:“未将对象引用设置到对象的实例。”
                catch { }
                await wx.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => { callback(m, dt.IsOwn); }));
                dt.Data.Add(m);
            }
            return dt;
        }
        public async Task<SortedDictionary<string, MusicGData>> GetGdListAsync()
        {
            var dt = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/rsc/fcgi-bin/fcg_get_profile_homepage.fcg?loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&cid=205360838&ct=20&userid={Settings.USettings.LemonAreeunIts}&reqfrom=1&reqtype=0");
            var o = JObject.Parse(dt);
            var data = new SortedDictionary<string, MusicGData>();
            var dx = o["data"]["mydiss"]["list"];
            foreach (var ex in dx)
            {
                var df = new MusicGData();
                df.id = ex["dissid"].ToString();
                df.name = ex["title"].ToString();
                if (ex["picurl"].ToString() != "")
                    df.pic = ex["picurl"].ToString();
                else df.pic = "https://y.gtimg.cn/mediastyle/global/img/cover_playlist.png?max_age=31536000";
                data.Add(df.id, df);
            }
            return data;
        }
        public async Task<SortedDictionary<string, MusicGData>> GetGdILikeListAsync()
        {
            var dt = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/fav/fcgi-bin/fcg_get_profile_order_asset.fcg?g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&ct=20&cid=205360956&userid={Settings.USettings.LemonAreeunIts}&reqtype=3&sin=0&ein=25");
            var o = JObject.Parse(dt);
            var data = new SortedDictionary<string, MusicGData>();
            var dx = o["data"]["cdlist"];
            foreach (var ex in dx)
            {
                var df = new MusicGData();
                df.id = ex["dissid"].ToString();
                df.name = ex["dissname"].ToString();
                if (ex["logo"].ToString() != "")
                    df.pic = ex["logo"].ToString();
                else df.pic = "https://y.gtimg.cn/mediastyle/global/img/cover_playlist.png?max_age=31536000";
                data.Add(df.id, df);
            }
            return data;
        }
        public static async Task<string> GetUrlAsync(string Musicid)
        {
            List<String[]> MData = new List<String[]>();
            MData.Add(new String[] { "M800", "mp3" });
            MData.Add(new String[] { "C600", "m4a" });
            MData.Add(new String[] { "M500", "mp3" });
            MData.Add(new String[] { "C400", "m4a" });
            MData.Add(new String[] { "M200", "mp3" });
            MData.Add(new String[] { "M100", "mp3" });

            var guid = "365305415";
            var mid = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg?songmid={Musicid}&platform=yqq&format=json"))["data"][0]["file"]["media_mid"].ToString();
            for (int i = 0; i < MData.Count; i++)
            {
                String[] datakey = MData[i];
                var key = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/base/fcgi-bin/fcg_musicexpress.fcg?json=3&guid={guid}&format=json"))["key"].ToString();
                string uri = $"https://dl.stream.qqmusic.qq.com/{datakey[0]}{mid}.{datakey[1]}?vkey={key}&guid={guid}&uid=0&fromtag=30";
                if (await HttpHelper.GetWebCode(uri) == 200)
                    return uri;
            }
            return "http://ws.stream.qqmusic.qq.com/C100" + mid + ".m4a?fromtag=0&guid=" + guid;
        }
        public async void GetAndPlayMusicUrlAsync(string mid, Boolean openlyric, Run x, Window s, string songname, bool doesplay = true)
        {
            string downloadpath = Settings.USettings.CachePath + "Music\\" + mid + ".mp3";
            if (!File.Exists(downloadpath))
            {
                string musicurl = "";
                musicurl = await GetUrlAsync(mid);
                WebClient dc = new WebClient();
                dc.DownloadFileCompleted += delegate
                {
                    var fm = File.Open(downloadpath, FileMode.Open);
                    if (fm.Length != 0)
                    {
                        fm.Close();
                        fm.Dispose();
                        mp.Open(new Uri(downloadpath));
                        if (doesplay)
                            mp.Play();
                        s.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
                        {
                            x.Text = TextHelper.XtoYGetTo("[" + songname, "[", " -", 0).Replace("Wy", "");
                        }));
                    }
                    else
                    {
                        fm.Close();
                        fm.Dispose();
                        File.Delete(downloadpath);
                        GetAndPlayMusicUrlAsync(mid, openlyric, x, s, songname, doesplay);
                    }
                };
                dc.DownloadFileAsync(new Uri(musicurl), downloadpath);
                dc.DownloadProgressChanged += delegate (object sender, DownloadProgressChangedEventArgs e)
                {
                    s.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
                    {
                        x.Text = "加载中..." + e.ProgressPercentage + "%";
                    }));
                };
            }
            else
            {
                var fm = File.Open(downloadpath, FileMode.Open);
                if (fm.Length != 0)
                {
                    fm.Close();
                    fm.Dispose();
                    mp.Open(new Uri(downloadpath));
                    if (doesplay)
                        mp.Play();
                    s.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Windows.Forms.MethodInvoker(delegate ()
                    {
                        x.Text = TextHelper.XtoYGetTo("[" + songname, "[", " -", 0).Replace("Wy", "");
                    }));
                }
                else
                {
                    fm.Close();
                    fm.Dispose();
                    File.Delete(downloadpath);
                    GetAndPlayMusicUrlAsync(mid, openlyric, x, s, songname, doesplay);
                }
            }
            if (openlyric)
            {
                string dt = await GetLyric(mid);
                lv.LoadLrc(dt);
            }
        }
        public string PushLyric(string t, string x, string file)
        {
            List<string> datatime = new List<string>();
            List<string> datatext = new List<string>();
            Dictionary<string, string> gcdata = new Dictionary<string, string>();
            string[] dta = t.Split('\n');
            foreach (var dt in dta)
                LyricView.parserLine(dt, datatime, datatext, gcdata);
            List<String> dataatimes = new List<String>();
            List<String> dataatexs = new List<String>();
            Dictionary<String, String> fydata = new Dictionary<String, String>();
            String[] dtaa = x.Split('\n');
            foreach (var dt in dtaa)
                LyricView.parserLine(dt, dataatimes, dataatexs, fydata);
            List<String> KEY = new List<String>();
            Dictionary<String, String> gcfydata = new Dictionary<String, String>();
            Dictionary<String, String> list = new Dictionary<String, String>();
            foreach (var dt in datatime)
            {
                KEY.Add(dt);
                gcfydata.Add(dt, "");
            }
            for (int i = 0; i != gcfydata.Count; i++)
            {
                if (fydata.ContainsKey(KEY[i]))
                    gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^" + fydata[KEY[i]]).Replace("\n", "").Replace("\r", "");
                else
                {
                    string dt = LyricView.YwY(KEY[i], 1);
                    if (fydata.ContainsKey(dt))
                        gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^" + fydata[dt]).Replace("\n", "").Replace("\r", "");
                    else gcfydata[KEY[i]] = (gcdata[KEY[i]] + "^").Replace("\n", "").Replace("\r", "");
                }
            }
            string LyricData = "";
            for (int i = 0; i != KEY.Count; i++)
            {
                String value = gcfydata[KEY[i]].Replace("[", "").Replace("]", "");
                String key = KEY[i];
                LyricData += $"[{key}]{value}\r\n";
            }
            File.WriteAllText(file, LyricData);
            return LyricData;
        }
        public async Task<string> GetLyric(string McMind)
        {
            string file = Settings.USettings.CachePath + "Lyric\\" + McMind + ".lrc";
            if (!File.Exists(file))
            {
                WebClient c = new WebClient();
                c.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36");
                c.Headers.Add("Accept", "*/*");
                c.Headers.Add("Referer", "https://y.qq.com/portal/player.html");
                c.Headers.Add("Accept-Language", "zh-CN,zh;q=0.8");
                c.Headers.Add("Cookie", $"tvfe_boss_uuid=c3db0dcc4d677c60; pac_uid=1_{Settings.USettings.LemonAreeunIts}; qq_slist_autoplay=on; ts_refer=ADTAGh5_playsong; RK=pKOOKi2f1O; pgv_pvi=8927113216; o_cookie={Settings.USettings.LemonAreeunIts}; pgv_pvid=5107924810; ptui_loginuin={Settings.USettings.LemonAreeunIts}; ptcz=897c17d7e17ae9009e018ebf3f818355147a3a26c6c67a63ae949e24758baa2d; pt2gguin=o{Settings.USettings.LemonAreeunIts}; pgv_si=s5715204096; qqmusic_fromtag=66; yplayer_open=1; ts_last=y.qq.com/portal/player.html; ts_uid=996779984; yq_index=0");
                c.Headers.Add("Host", "c.y.qq.com");
                string s = TextHelper.XtoYGetTo(c.DownloadString($"https://c.y.qq.com/lyric/fcgi-bin/fcg_query_lyric_new.fcg?callback=MusicJsonCallback_lrc&pcachetime=1494070301711&songmid={McMind}&g_tk=5381&jsonpCallback=MusicJsonCallback_lrc&loginUin=0&hostUin=0&format=jsonp&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"), "MusicJsonCallback_lrc(", ")", 0);
                Console.WriteLine(s);
                JObject o = JObject.Parse(s);
                string t = Encoding.UTF8.GetString(Convert.FromBase64String(o["lyric"].ToString())).Replace("&apos;", "\'");
                if (o["trans"].ToString() == "") { await Task.Run(() => { File.WriteAllText(file, t); }); return t; }
                else
                {
                    string x = Encoding.UTF8.GetString(Convert.FromBase64String(o["trans"].ToString())).Replace("&apos;", "\'");
                    return PushLyric(t, x, file);
                }
            }
            else
                return File.ReadAllText(file);
        }
        public async Task<List<MusicTop>> GetTopIndexAsync()
        {
            var dt = await HttpHelper.GetWebAsync("https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_opt.fcg?page=index&format=html&tpl=macv4&v8debug=1");
            var sh = "{\"data\":" + dt.Replace("jsonCallback(", "").Replace("}]\n)", "") + "}]" + "}";
            var o = JObject.Parse(sh);
            var data = new List<MusicTop>();
            int i = 0;
            var d0l = o["data"][0]["List"];
            while (i < d0l.Count())
            {
                var d0li = d0l[i];
                data.Add(new MusicTop
                {
                    Name = d0li["ListName"].ToString(),
                    Photo = d0li["pic_v12"].ToString(),
                    ID = d0li["topID"].ToString()
                });
                i++;
            }
            i = 0;
            while (i < o["data"][1]["List"].Count())
            {
                data.Add(new MusicTop
                {
                    Name = o["data"][1]["List"][i]["ListName"].ToString(),
                    Photo = o["data"][1]["List"][i]["pic_v12"].ToString(),
                    ID = o["data"][1]["List"][i]["topID"].ToString()
                });
                i++;
            }
            return data;
        }
        public async Task<List<Music>> GetToplistAsync(string TopID, int osx = 1)
        {
            int index = (osx - 1) * 30;
            JObject o = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/v8/fcg-bin/fcg_v8_toplist_cp.fcg?tpl=3&page=detail&topid={TopID}&type=top&song_begin={index}&song_num=30&g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"));
            List<Music> dt = new List<Music>();
            int i = 0;
            var s = o["songlist"];
            while (i < s.Count())
            {
                var sid = s[i]["data"];
                Music m = new Music();
                m.MusicName = sid["songname"].ToString();
                m.MusicName_Lyric = sid["albumdesc"].ToString();
                string Singer = "";
                List<MusicSinger> lm = new List<MusicSinger>();
                for (int osxc = 0; osxc != sid["singer"].Count(); osxc++)
                {
                    Singer += sid["singer"][osxc]["name"] + "&";
                    lm.Add(new MusicSinger() { Name = sid["singer"][osxc]["name"].ToString(),
                        Mid = sid["singer"][osxc]["mid"].ToString()
                    });
                }
                m.Singer = lm;
                m.SingerText = Singer.Substring(0, Singer.LastIndexOf("&"));
                m.MusicID = sid["songmid"].ToString();
                var amid = sid["albummid"].ToString();
                if (amid == "001ZaCQY2OxVMg")
                    m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T001R300x300M000{sid["singer"][0]["mid"].ToString()}.jpg?max_age=2592000";
                else m.ImageUrl = $"https://y.gtimg.cn/music/photo_new/T002R300x300M000{amid}.jpg?max_age=2592000";
                dt.Add(m);
                i++;
            }
            return dt;
        }
        public async Task<List<MusicSinger>> GetSingerAsync(string key, int page = 1)
        {
            var o = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/v8/fcg-bin/v8.fcg?channel=singer&page=list&key={key}&pagesize=100&pagenum={page}&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"));
            var data = new List<MusicSinger>();
            int i = 0;
            var dl = o["data"]["list"];
            while (i < dl.Count())
            {
                var dli = dl[i];
                data.Add(new MusicSinger
                {
                    Name = dli["Fsinger_name"].ToString(),
                    Mid=dli["Fsinger_mid"].ToString(),
                    Photo = $"https://y.gtimg.cn/music/photo_new/T001R150x150M000{dli["Fsinger_mid"]}.jpg?max_age=2592000"
                });
                i++;
            }
            return data;
        }
        public async Task<MusicFLGDIndexItemsList> GetFLGDIndexAsync()
        {
            var o = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/splcloud/fcgi-bin/fcg_get_diss_tag_conf.fcg?g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0", Encoding.UTF8));
            var data = new MusicFLGDIndexItemsList();
            data.Hot.Add(new MusicFLGDIndexItems { id = "10000000", name = "全部" });
            int i = 0;
            var dc = o["data"]["categories"];
            while (i < dc[1]["items"].Count())
            {
                var dci = dc[1]["items"][i];
                data.Lauch.Add(new MusicFLGDIndexItems
                {
                    id = dci["categoryId"].ToString(),
                    name = dci["categoryName"].ToString()
                });
                i++;
            }
            i = 0;
            while (i < dc[2]["items"].Count())
            {
                var dci = dc[2]["items"][i];
                data.LiuPai.Add(new MusicFLGDIndexItems
                {
                    id = dci["categoryId"].ToString(),
                    name = dci["categoryName"].ToString()
                });
                i++;
            }
            i = 0;
            while (i < dc[3]["items"].Count())
            {
                var dci = dc[3]["items"][i];
                data.Theme.Add(new MusicFLGDIndexItems
                {
                    id = dci["categoryId"].ToString(),
                    name = dci["categoryName"].ToString()
                });
                i++;
            }
            i = 0;
            while (i < dc[4]["items"].Count())
            {
                var dci = dc[4]["items"][i];
                data.Heart.Add(new MusicFLGDIndexItems
                {
                    id = dci["categoryId"].ToString(),
                    name = dci["categoryName"].ToString()
                });
                i++;
            }
            i = 0;
            while (i < dc[5]["items"].Count())
            {
                var dci = dc[5]["items"][i];
                data.Changjing.Add(new MusicFLGDIndexItems
                {
                    id = dci["categoryId"].ToString(),
                    name = dci["categoryName"].ToString()
                });
                i++;
            }
            return data;
        }
        public async Task<List<MusicGD>> GetFLGDAsync(string id,int osx=1)
        {
            int start = (osx - 1) * 30;
            int end = start + 29;
            var o = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/splcloud/fcgi-bin/fcg_get_diss_by_tag.fcg?picmid=1&rnd=0.38615680484561965&g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&categoryId={id}&sortId=5&sin={start}&ein={end}", Encoding.UTF8));
            var data = new List<MusicGD>();
            int i = 0;
            var dl = o["data"]["list"];
            while (i < dl.Count())
            {
                var dli = dl[i];
                data.Add(new MusicGD
                {
                    Name = dli["dissname"].ToString(),
                    Photo = dli["imgurl"].ToString(),
                    ID = dli["dissid"].ToString()
                });
                i++;
            }
            return data;
        }
        public async Task<MusicRadioList> GetRadioList()
        {
            var o = JObject.Parse(await HttpHelper.GetWebAsync("https://c.y.qq.com/v8/fcg-bin/fcg_v8_radiolist.fcg?channel=radio&format=json&page=index&tpl=wk&new=1&p=0.8663229811059507&g_tk=5381&loginUin=0&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"));
            var data = new MusicRadioList();
            var ddg = o["data"]["data"]["groupList"];
            try
            {
                int i = 0;
                while (i < ddg[0]["radioList"].Count())
                {
                    var ddgri = ddg[0]["radioList"][i];
                    data.Hot.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                }
                i = 0;
                while (i < ddg[1]["radioList"].Count())
                {
                    var ddgri = ddg[1]["radioList"][i];
                    data.Evening.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                }
                i = 0;
                while (i < ddg[2]["radioList"].Count())
                {
                    var ddgri = ddg[2]["radioList"][i];
                    data.Love.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                }
                i = 0;
                while (i < ddg[3]["radioList"].Count())
                {
                    var ddgri = ddg[3]["radioList"][i];
                    data.Theme.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                }
                i = 0;
                while (i < ddg[4]["radioList"].Count())
                {
                    var ddgri = ddg[4]["radioList"][i];
                    data.Changjing.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
                i = 0;
                while (i < ddg[5]["radioList"].Count())
                {
                    var ddgri = ddg[4]["radioList"][i];
                    data.Style.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
                i = 0;
                while (i < ddg[6]["radioList"].Count())
                {
                    var ddgri = ddg[6]["radioList"][i];
                    data.Lauch.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
                i = 0;
                while (i < ddg[7]["radioList"].Count())
                {
                    var ddgri = ddg[7]["radioList"][i];
                    data.People.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
                i = 0;
                while (i < ddg[8]["radioList"].Count())
                {
                    var ddgri = ddg[8]["radioList"][i];
                    data.MusicTools.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
                i = 0;
                while (i < ddg[9]["radioList"].Count())
                {
                    var ddgri = ddg[9]["radioList"][i];
                    data.Diqu.Add(new MusicRadioListItem
                    {
                        Name = ddgri["radioName"].ToString(),
                        Photo = ddgri["radioImg"].ToString(),
                        ID = ddgri["radioId"].ToString()
                    });
                    i++;
                    i++;
                }
            }
            catch { }
            return data;
        }
        public async Task<Music> GetRadioMusicAsync(string id)
        {
            if (id == "99")
            {
                var o = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/rcmusic2/fcgi-bin/fcg_guess_youlike_pc.fcg?g_tk=1206122277&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&cid=703&uin={Settings.USettings.LemonAreeunIts}"));
                string Singer = "";
                var s_0 = o["songlist"][0];
                var s_0_s = o["songlist"][0]["singer"];
                List<MusicSinger> lm = new List<MusicSinger>();
                for (int osxc = 0; osxc != s_0_s.Count(); osxc++){
                    Singer += s_0_s[osxc]["name"] + "&";
                    lm.Add(new MusicSinger(){
                        Name= s_0_s[osxc]["name"].ToString(),
                        Mid= s_0_s[osxc]["mid"].ToString()
                    });
                }
                var data = new Music
                {
                    MusicName = s_0["name"].ToString(),
                    SingerText = Singer.Substring(0, Singer.LastIndexOf("&")),
                    Singer = lm,
                    MusicID = s_0["mid"].ToString(),
                    ImageUrl = $"http://y.gtimg.cn/music/photo_new/T002R300x300M000{s_0["album"]["mid"]}.jpg"
                };
                return data;
            }
            else
            {
                var o = JObject.Parse(await HttpHelper.GetWebAsync($"https://u.y.qq.com/cgi-bin/musicu.fcg?g_tk=1206122277&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0&data=%7B\"songlist\"%3A%7B\"module\"%3A\"pf.radiosvr\"%2C\"method\"%3A\"GetRadiosonglist\"%2C\"param\"%3A%7B\"id\"%3A{id}%2C\"firstplay\"%3A1%2C\"num\"%3A10%7D%7D%2C\"radiolist\"%3A%7B\"module\"%3A\"pf.radiosvr\"%2C\"method\"%3A\"GetRadiolist\"%2C\"param\"%3A%7B\"ct\"%3A\"24\"%7D%7D%2C\"comm\"%3A%7B\"ct\"%3A\"24\"%7D%7D"));
                string Singer = "";
                var sdt0 = o["songlist"]["data"]["track_list"][0];
                var sdt0s = sdt0["singer"];
                List<MusicSinger> lm = new List<MusicSinger>();
                for (int osxc = 0; osxc != sdt0s.Count(); osxc++){
                    Singer += sdt0s[osxc]["name"] + "&";
                    lm.Add(new MusicSinger() {
                        Name= sdt0s[osxc]["name"].ToString(),
                        Mid= sdt0s[osxc]["mid"].ToString()
                    });
                }
                var data = new Music
                {
                    MusicName = sdt0["name"].ToString(),
                    SingerText = Singer.Substring(0, Singer.LastIndexOf("&")),
                    Singer=lm,
                    MusicID = sdt0["mid"].ToString(),
                    ImageUrl = $"http://y.gtimg.cn/music/photo_new/T002R300x300M000{sdt0["album"]["mid"]}.jpg"
                };
                return data;
            }
        }
        public async void GetGDbyWYAsync(string id, Window x, TextBlock tb, ProgressBar pb, Action Finished)
        {
            string data = await HttpHelper.GetWebAsync($"http://music.163.com/api/playlist/detail?id={id}&updateTime=-1");
            JObject o = JObject.Parse(data);
            var dt = new MusicGData();
            string ids = "";
            string typelist = "";
            var pl = o["result"];
            dt.name = pl["name"].ToString();
            dt.id = pl["id"].ToString();
            dt.pic = pl["coverImgUrl"].ToString();
            var pl_t = pl["tracks"];
            x.Dispatcher.Invoke(() => { pb.Maximum = pl_t.Count(); });
            int i = 1;
            foreach (var pl_t_i in pl_t)
            {
                var dtname = pl_t_i["name"].ToString();
                var dtsinger = "";
                var pl_t_i_ar = pl_t_i["artists"];
                for (int dx = 0; dx != pl_t_i_ar.Count(); dx++)
                    dtsinger += pl_t_i_ar[0]["name"] + "&";
                dtsinger = dtsinger.Substring(0, dtsinger.LastIndexOf("&"));
                var dtf = await SearchMusicAsync(dtname + "-" + dtsinger);
                if (dtf.Count > 0)
                {
                    var dtv = dtf[0];
                    dt.Data.Add(dtv);
                    ids += dtv.MusicID + ",";
                    typelist += "13,";
                    x.Dispatcher.Invoke(() => { pb.Value = i; tb.Text = dtv.MusicName + " - " + dtv.Singer; });
                }
                else x.Dispatcher.Invoke(() => { pb.Value--; });
                i++;
            }
            ids = ids.Substring(0, ids.LastIndexOf(","));
            typelist = typelist.Substring(0, typelist.LastIndexOf(","));
            Console.WriteLine("ids:" + ids);
            AddNewGd(dt.name);
            await Task.Delay(500);
            string dir = await GetGDdiridByNameAsync(dt.name);
            Console.WriteLine("dirId" + dir);
            var amt = AddMusicToGDPL(ids, dir, typelist);
            Console.WriteLine(amt[0] + amt[1]);
            x.Dispatcher.Invoke(() =>
            {
                Finished();
            });
        }
        public async Task<List<MusicPL>> GetPLAsync(string name, int page = 1)
        {
            string Page = ((page - 1) * 20).ToString();
            string id = GetWYIdByName(name);
            var data = await HttpHelper.GetWebAsync($"https://music.163.com/api/v1/resource/comments/R_SO_4_{id}?offset={Page}");
            JObject o = JObject.Parse(data);
            var d = new List<MusicPL>();
            var hc = o["hotComments"];
            for (int i = 0; i != hc.Count(); i++)
            {
                var hc_i = o["hotComments"][i];
                var hc_i_u = hc_i["user"];
                d.Add(new MusicPL()
                {
                    text = hc_i["content"].ToString(),
                    name = hc_i_u["nickname"].ToString(),
                    img = hc_i_u["avatarUrl"].ToString(),
                    like = hc_i["likedCount"].ToString()
                });
            }
            return d;
        }
        public async Task<List<MusicPL>> GetPLByQQAsync(string mid)
        {
            string id = JObject.Parse(await HttpHelper.GetWebAsync($"https://c.y.qq.com/v8/fcg-bin/fcg_play_single_song.fcg?songmid={mid}&tpl=yqq_song_detail&format=json&g_tk=268405378&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq&needNewCode=0"))["data"][0]["id"].ToString();
            string dt = await HttpHelper.GetWebAsync($"https://c.y.qq.com/base/fcgi-bin/fcg_global_comment_h5.fcg?g_tk=642290724&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=GB2312&notice=0&platform=yqq&needNewCode=0&cid=205360772&reqtype=2&biztype=1&topid={id}&cmd=8&needmusiccrit=0&pagenum=0&pagesize=25&lasthotcommentid=&domain=qq.com&ct=24&cv=101010");
            JObject ds = JObject.Parse(dt.Replace("\n", ""));
            List<MusicPL> data = new List<MusicPL>();
            JToken hcc = ds["hot_comment"]["commentlist"];
            for (int i = 0; i != hcc.Count(); i++)
            {
                JToken hcc_i = ds["hot_comment"]["commentlist"][i];
                MusicPL mpl = new MusicPL(){
                    img =hcc_i["avatarurl"].ToString(),
                    like = hcc_i["praisenum"].ToString(),
                    name = hcc_i["nick"].ToString(),
                    text =TextHelper.Exem(hcc_i["rootcommentcontent"].ToString().Replace(@"\n", "\n")),
                    commentid = hcc_i["commentid"].ToString()
                };
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(hcc_i["time"].ToString() + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime daTime = dtStart.Add(toNow);
                mpl.time = daTime.ToString("yyyy-MM-dd  HH:mm");
                if (hcc_i["ispraise"].ToString() == "1")
                    mpl.ispraise = true;
                else mpl.ispraise = false;
                data.Add(mpl);
            }
            return data;
        }
        public string GetWYIdByName(string name)
        {
            var ds = "{\"data\":" + HttpHelper.PostWeb("http://lab.mkblog.cn/music/api.php", "types=search&count=20&source=netease&pages=1&name=" + Uri.EscapeDataString(name),HttpHelper.GetWebHeader_MKBlog()) + "}";
            var s = JObject.Parse(ds);
            return s["data"][0]["id"].ToString();
        }
        public static string[] AddMusicToGD(string id, string dirid)
        {
            Console.WriteLine(Settings.USettings.Cookie + "   " + Settings.USettings.g_tk);
            string result = HttpHelper.PostWeb("https://c.y.qq.com/splcloud/fcgi-bin/fcg_music_add2songdir.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.post&needNewCode=0&uin={Settings.USettings.LemonAreeunIts}&midlist={id}&typelist=13&dirid={dirid}&addtype=&formsender=4&source=153&r2=0&r3=1&utf8=1&g_tk=" + Settings.USettings.g_tk, HttpHelper.GetWebHeader_YQQCOM());
            //添加本地缓存
            JObject o = JObject.Parse(result);
            string msg = o["msg"].ToString();
            string title = o["title"].ToString();
            return new string[2] { msg, title };
        }
        public static string[] AddMusicToGDPL(string ids, string dirid, string typelist)
        {
            Console.WriteLine(Settings.USettings.Cookie + "   " + Settings.USettings.g_tk);
            string result = HttpHelper.PostWeb("https://c.y.qq.com/splcloud/fcgi-bin/fcg_music_add2songdir.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.post&needNewCode=0&uin={Settings.USettings.LemonAreeunIts}&midlist={ids}&typelist={typelist}&dirid={dirid}&addtype=&formsender=4&source=153&r2=0&r3=1&utf8=1&g_tk=" + Settings.USettings.g_tk, HttpHelper.GetWebHeader_YQQCOM());
            //添加本地缓存
            JObject o = JObject.Parse(result);
            string msg = o["msg"].ToString();
            string title = o["title"].ToString();
            return new string[2] { msg, title };
        }
        public static string DeleteMusicFromGD(string Musicid, string Dissid, string dirid)
        {
            string result = HttpHelper.PostWeb("https://c.y.qq.com/qzone/fcg-bin/fcg_music_delbatchsong.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.post&needNewCode=0&uin={Settings.USettings.LemonAreeunIts}&dirid={dirid}&ids={Musicid}&source=103&types=3&formsender=4&flag=2&from=3&utf8=1&g_tk=" + Settings.USettings.g_tk, HttpHelper.GetWebHeader_YQQCOM());
            string ok = JObject.Parse(result)["msg"].ToString();
            return ok;
        }
        public static async Task<string> GetGDdiridByNameAsync(string name)
        {
            Console.WriteLine(Settings.USettings.LemonAreeunIts);
            JObject o = JObject.Parse(await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/splcloud/fcgi-bin/songlist_list.fcg?utf8=1&-=MusicJsonCallBack&uin={Settings.USettings.LemonAreeunIts}&rnd=0.693477705380313&g_tk={Settings.USettings.g_tk}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=utf-8&notice=0&platform=yqq.json&needNewCode=0"));
            foreach (var a in o["list"])
            {
                string st = HttpUtility.HtmlDecode(a["dirname"].ToString());
                Console.WriteLine(st);
                if (name == st)
                    return a["dirid"].ToString();
            }
            return "null";
        }
        public static string AddGDILike(string dissid)
        {
            string result = HttpHelper.PostWeb("https://c.y.qq.com/folder/fcgi-bin/fcg_qm_order_diss.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=fs&inCharset=GB2312&outCharset=utf8&notice=0&platform=yqq&needNewCode=0&g_tk={Settings.USettings.g_tk}&uin={Settings.USettings.LemonAreeunIts}&dissid={dissid}&from=1&optype=1&utf8=1&qzreferrer=https%3A%2F%2Fy.qq.com%2Fn%2Fyqq%2Fplaysquare%2F{dissid}.html%23stat%3Dy_new.playlist.pic_click", HttpHelper.GetWebHeader_YQQCOM());
            return result;
        }
        public static string DelGDILike(string dissid) {
            string result = HttpHelper.PostWeb("https://c.y.qq.com/folder/fcgi-bin/fcg_qm_order_diss.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=fs&inCharset=GB2312&outCharset=gb2312&notice=0&platform=yqq&needNewCode=0&g_tk={Settings.USettings.g_tk}&uin={Settings.USettings.LemonAreeunIts}&ordertype=0&optype=2&dissid={dissid}&from=1", HttpHelper.GetWebHeader_YQQCOM());
            return result;
        }
        public static string AddNewGd(string name)
        {
            string result = HttpHelper.PostWeb("https://c.y.qq.com/splcloud/fcgi-bin/create_playlist.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=fs&inCharset=GB2312&outCharset=utf8&notice=0&platform=yqq&needNewCode=0&g_tk={Settings.USettings.g_tk}&uin={Settings.USettings.LemonAreeunIts}&name={HttpUtility.UrlEncode(name)}&description=&show=1&pic_url=&tags=&tagids=&formsender=1&utf8=1&qzreferrer=https%3A%2F%2Fy.qq.com%2Fportal%2Fprofile.html%23sub%3Dother%26tab%3Dcreate%26stat%3Dy_new.top.user_pic", HttpHelper.GetWebHeader_YQQCOM());
            return result;
        }
        public static string DeleteGdById(string dirid) {
            string result = HttpHelper.PostWeb("https://c.y.qq.com/splcloud/fcgi-bin/fcg_fav_modsongdir.fcg?g_tk=" + Settings.USettings.g_tk,
                $"loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=fs&inCharset=GB2312&outCharset=gb2312&notice=0&platform=yqq&needNewCode=0&g_tk={Settings.USettings.g_tk}&uin={Settings.USettings.LemonAreeunIts}&delnum=1&deldirids={dirid}&forcedel=1&formsender=1&source=103", HttpHelper.GetWebHeader_YQQCOM());
            return result;
        }
        public static async Task<string> GetMusicIdByMidAsync(string mid) {
            string st =(await HttpHelper.GetWebAsync($"https://y.qq.com/n/yqq/song/{mid}.html")).Replace(" ", "").Replace("\r\n", "");
            string json = TextHelper.XtoYGetTo(st, "<script>varg_SongData=", ";</script>", 0);
            Console.WriteLine(json);

            JObject o = JObject.Parse(json);
            return o["songid"].ToString(); ;
        }
        public static async Task<string> PraiseMusicPLAsync(string mid,MusicPL mp) {
            string id = await GetMusicIdByMidAsync(mid);
            string get = "";
            if (mp.ispraise)
                get = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/base/fcgi-bin/fcg_global_comment_praise_h5.fcg?g_tk={Settings.USettings.LemonAreeunIts}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=GB2312&notice=0&platform=yqq.json&needNewCode=0&cid=205360774&cmd=2&reqtype=2&biztype=1&topid={id}&commentid={mp.commentid}&qq={Settings.USettings.LemonAreeunIts}&domain=qq.com&ct=24&cv=101010");
            else get = await HttpHelper.GetWebDatacAsync($"https://c.y.qq.com/base/fcgi-bin/fcg_global_comment_praise_h5.fcg?g_tk={Settings.USettings.LemonAreeunIts}&loginUin={Settings.USettings.LemonAreeunIts}&hostUin=0&format=json&inCharset=utf8&outCharset=GB2312&notice=0&platform=yqq.json&needNewCode=0&cid=205360774&cmd=1&reqtype=2&biztype=1&topid={id}&commentid={mp.commentid}&qq={Settings.USettings.LemonAreeunIts}&domain=qq.com&ct=24&cv=101010");
            return get;
        }
    }
}
