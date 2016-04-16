using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using SAT.Util;
using System.Drawing;


namespace LibTest {

    public class MyClass {
        public bool PropBool {
            get;
            internal set;
        }
        public string PropString {
            get;
            internal set;
        }
        public IList ie {
            get;
            set;
        }
    }


    class Program {
        static void Main(string[] args) {
            //JsonTest();
            DeviceTest();
        }
        private static void DeviceTest() {
            var list = USBDeviceUtil.GetDevices();
            var json = new JsonWriter();
            json.ReadableFormat = true;
            var str = json.Encode(list);
            Console.WriteLine(str);

            if (USBDeviceUtil.IsDeviceConnected(@"USB\\VID_167C&PID_0032\\%")) {
                Console.WriteLine("Camera is connected.");
            } else {
                Console.WriteLine("Camera is DisConnected.");
            }


            Console.ReadLine();
        }

        private static void JsonTest() {
            var rect = new Rectangle();
            rect.X = 100;
            rect.Y = 200;
            rect.Width = 50;
            rect.Height = 150;
            var json = new JsonWriter();
            json.ReadableFormat = true;
            var dic = new Dictionary<string, object>();
            dic["Foo"] = "ばー";
            dic["ばず"] = "baz!\"\n改行付き\n文字列";
            dic["rect"] = rect;
            dic["長さ"] = 1000;
            var mc = new MyClass();
            var ie = new List<object>();
            ie.Add(10);
            ie.Add(1000);
            ie.Add(200);
            ie.Add("文字列");

            mc.ie = ie;

            mc.PropBool = false;

            mc.PropString = "プロパティ文字列";
            dic["mc"] = mc;
            


            string str = json.Encode(dic);
            Console.WriteLine(str);
            Console.ReadLine();
        }
    }
}
