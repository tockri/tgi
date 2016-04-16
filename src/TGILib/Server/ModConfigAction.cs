using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Reflection;
using TGILib;
using SAT.Util;
using System.IO;

namespace TGILib.Server {
    /// <summary>
    /// configの処理
    /// </summary>
    class ModConfigAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            Config c;
            
            if (gets["DirName"] != null) {
                c = app.GetConfig(gets["DirName"]);
            } else {
                c = app.CurrentConfig;
            }
            // string型のプロパティ
            var textKeys = new string[] {
                "SiteName", "SitePerson", "SheetType", "FusionTemp", "FusionSpeed",
                "FusionPressure", "Memo"
            };
            foreach (var tk in textKeys) {
                if (gets[tk] != null) {
                    var pi = c.GetType().GetProperty(tk);
                    pi.SetValue(c, gets[tk], null);
                }
            }
            // bool型のプロパティ
            var bookKeys = new string[] {
                "CsvOutput", "ImageOutput", "AlermSound"
            };
            foreach (var bk in bookKeys) {
                if (gets[bk] != null) {
                    int iv = 0;
                    if (int.TryParse(gets[bk], out iv)) {
                        var pi = c.GetType().GetProperty(bk);
                        pi.SetValue(c, iv != 0, null);
                    } else {
                        ErrorMessage = bk + "の値が不正です。: " + iv;
                        return false;
                    }
                }
            }
            // 閾値判定用変数
            if (gets["ThresholdSet"] != null) {
                var v = gets["ThresholdSet"];
                var sets = app.GetThresholdSets();
                if (sets[v] != null) {
                    c.ThresholdSlope = sets[v].Slope;
                    c.ThresholdIntercept = sets[v].Intercept;
                } else {
                    ErrorMessage = "ThresholdSetの値が不正です。: " + v;
                    return false;
                }
            }
            // カラーマップ
            if (gets["ColorMap"] != null) {
                var v = gets["ColorMap"];
                try {
                    c.ColorMapFocusType = (ColorMapMaker.FocusType)Enum.Parse(typeof(ColorMapMaker.FocusType), v);
                    app.FireClientEvent(TGIApp.ClientEventType.ColorMap, c.ColorMapFocusType.ToString());
                } catch (Exception e) {
                    ErrorMessage = e.Message;
                    return false;
                }
            }
            // cofファイル
            if (gets["CofFile"] != null) {
                var v = gets["CofFile"];
                var files = app.GetCofFileNames();
                var fileName = files[v];
                if (fileName != null) {
                    c.CofFilePath = Path.Combine(AppConfig.CofRoot, fileName);
                } else {
                    ErrorMessage = "CofFileの値が不正です。: " + v;
                    return false;
                }
            }
            c.Save();
            // Currentの場合は即座に反映する
            if (c == app.CurrentConfig) {
                app.LoadSiteConfig();
            }
            return true;
        }
    }
}
