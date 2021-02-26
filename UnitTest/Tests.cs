using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Netch.Models;
using Netch.Servers.Shadowsocks;
using Netch.Servers.ShadowsocksR;
using Netch.Servers.VMess.Form;
using Netch.Utils;

namespace UnitTest
{
    [TestClass]
    public class Tests : TestBase
    {
        public static void TestServerForm()
        {
            i18N.Load("zh-CN");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new VMessForm());
        }

        [TestMethod]
        public void TestLoadSetting()
        {
            var s = new Server[] {new Shadowsocks(), new ShadowsocksR()};
            Console.WriteLine(JsonSerializer.Deserialize<List<Server>>(JsonSerializer.Serialize(s),
                new JsonSerializerOptions {Converters = {new ServerConverterWithTypeDiscriminator()}}));
        }
    }
}