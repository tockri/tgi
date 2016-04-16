using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    /// <summary>
    /// configの処理
    /// </summary>
    class ConfigAction : ApiAction {
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            Config c;
            if (gets["DirName"] != null) {
                c = app.GetConfig(gets["DirName"]);
            } else {
                c = app.CurrentConfig;
            }
            ResponseData["Values"] = ConfigValues(c);
            return true;
        }
        /// <summary>
        /// Configオブジェクトの内容を返す
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        internal static Dictionary<string, object> ConfigValues(Config c) {
            TGIApp app = TGIApp.Instance;
            var Values = new Dictionary<string, object>();
            Values["DirName"] = c.DirName;
            Values["SiteName"] = c.SiteName;
            Values["SitePerson"] = c.SitePerson;
            Values["SheetType"] = c.SheetType;
            Values["FusionTemp"] = c.FusionTemp;
            Values["FusionSpeed"] = c.FusionSpeed;
            Values["FusionPressure"] = c.FusionPressure;
            Values["Memo"] = c.Memo;
            Values["CsvOutput"] = c.CsvOutput ? 1 : 0;
            Values["ImageOutput"] = c.ImageOutput ? 1 : 0;
            Values["AlermSound"] = c.AlermSound ? 1 : 0;
            Values["ThresholdSet"] = app.GetThresholdSetKey(c.ThresholdSlope, c.ThresholdIntercept);
            Values["ColorMap"] = c.ColorMapFocusType.ToString();
            Values["CofFile"] = app.GetCofFileKey(c.CofFilePath);
            return Values;
        }
    }
}
