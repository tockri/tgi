using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using TGILib;

namespace TGILib.Server {
    class ModWatchboxAction : ApiAction{
        protected override bool InvokeInner(string method, NameValueCollection gets) {
            TGIApp app = TGIApp.Instance;
            // Keyの扱い
            string key = gets["Key"];
            if (key == null) {
                ErrorMessage = "Keyが指定されていません。";
                return false;
            }
            string[] keyElems = key.Split('-');
            if (keyElems.Length != 2 || (keyElems[0] != "main" && keyElems[0] != "sub")) {
                ErrorMessage = "不正なKey : " + key;
                return false;
            }
            // Valueの扱い
            int value = 0;
            if (!int.TryParse(gets["Value"], out value)) {
                ErrorMessage = "不正なValue: " + gets["Value"];
                return false;
            }
            // 変更対象
            var wbc = keyElems[0] == "main"
                    ? app.MainWatchBox.Config
                    : app.SubWatchBox.Config;

            // 変更箇所
            switch (keyElems[1]) {
                case "up":
                    wbc.Top -= value;
                    break;
                case "left":
                    wbc.Left -= value;
                    break;
                case "right":
                    wbc.Left += value;
                    break;
                case "down":
                    wbc.Top += value;
                    break;
                case "narrow":
                    wbc.Width -= value * 2;
                    wbc.Gap -= value * 2;
                    wbc.Left += value;
                    break;
                case "wide":
                    wbc.Width += value * 2;
                    wbc.Gap += value * 2;
                    wbc.Left -= value;
                    break;
                case "gapnarrow":
                    wbc.Gap += value * 2;
                    wbc.Left += value;
                    wbc.Width -= value * 2;
                    break;
                case "gapwide":
                    wbc.Gap -= value * 2;
                    wbc.Left -= value;
                    wbc.Width += value * 2;
                    break;
                case "double":
                    wbc.Mode = "Double";
                    break;
                case "single":
                    wbc.Mode = "Single";
                    break;
            }
            app.SaveWatchboxConfigs();
            // 返り値
            ResponseData["Main"] = app.MainWatchBox.Config;
            ResponseData["Sub"] = app.SubWatchBox.Config;
            return true;
        }
    }
}
