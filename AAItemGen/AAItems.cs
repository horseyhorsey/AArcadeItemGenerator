using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.IO;

namespace AAItemGen
{
    class AAItems
    {
        public string ItemFileName { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string FileLocation { get; set; }
        public string App { get; set; }
        public string MarqueeLocation { get; set; }
        public string ScreensLocation { get; set; }
        public string LastModel { get; set; }

        public List<AAItems> gamesList = new List<AAItems>();
       
        public void createJsonText(List<AAItems> list)
        {
            string endPath = string.Empty;

            foreach (AAItems item in list)
            {
                JObject rss =

                new JObject(
                new JProperty("title", item.Title),
                new JProperty("type", item.Type),
                new JProperty("filelocation", item.FileLocation),
                new JProperty("app", item.App),
                new JProperty("marqueeslocation", item.MarqueeLocation),
                new JProperty("screenslocation", item.ScreensLocation),
                new JProperty("lastmodel", item.LastModel));

                string jsonObjectString = rss.ToString();
                jsonObjectString = jsonObjectString.Insert(0, "item\n");
                jsonObjectString = jsonObjectString.Replace(",", string.Empty);
                jsonObjectString = jsonObjectString.Replace(@""":", @"""");
                jsonObjectString = jsonObjectString.Replace("null", @"""""");
                jsonObjectString = jsonObjectString.Replace(@"\\", @"\");

                createItemFile(item.ItemFileName, jsonObjectString);
            }

             //return jsonObjectString;
        }

        public void createItemFile(string fileName,string textToWrite)
        {
            // Check if file already exists. If yes, delete it. 
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            if (!Directory.Exists(fileName))
            {
                string folderName = System.IO.Path.GetDirectoryName(fileName);
                Directory.CreateDirectory(folderName);
            }

            // Create a new file 
            using (FileStream fs = File.Create(fileName))
            {
                // Add some text to file
                Byte[] title = new UTF8Encoding(true).GetBytes(textToWrite);
                fs.Write(title, 0, title.Length);
            }
        }

        public void generateItemsFromHS(string xmlPath, bool clonesOn,string RomFolder, string type,string ext,
                                        string app,string modelPath, string sysName,string AaPath, string screens="",string marquees="")
        {

            //I:\Roms\PC Games\SteamLibrary\SteamApps\common\Anarchy Arcade\aarcade\library\itemType
            XmlDocument xdoc = new XmlDocument();
            xdoc.Load(xmlPath);

            string name = string.Empty;
            string cloneOf = string.Empty;

            List<AAItems> gamesList = new List<AAItems>();

            foreach (XmlNode node in xdoc.SelectNodes("menu/game"))
            {
                name = node.SelectSingleNode("@name").InnerText;
                cloneOf = string.Empty;

                if (node.SelectSingleNode("cloneof") != null)
                    cloneOf = node.SelectSingleNode("cloneof").InnerText;

                string location = RomFolder + "\\" + name + "." + ext;
                string joinedName = name.Replace(" ", "");
                string endPath = System.IO.Path.Combine(AaPath, @"aarcade\library", type, joinedName + sysName + ".itm");

                if (!clonesOn)
                {
                    if (cloneOf == string.Empty)
                        gamesList.Add(new AAItems { Title = name, Type = type, FileLocation = location, App = app, LastModel = modelPath, ScreensLocation = screens + "\\" + name + ".png", MarqueeLocation = marquees + "\\" + name + ".png", ItemFileName = endPath });
                }
                else
                    gamesList.Add(new AAItems { Title = name, Type = type, FileLocation = location, App = app, LastModel = modelPath, ScreensLocation = screens + "\\" + name + ".png", MarqueeLocation = marquees + "\\" + name + ".png", ItemFileName = endPath });

           

            }

            createJsonText(gamesList);

        }

        public void generateItemsFromFolder(string RomFolder, string type,string ext,
                                        string app,string modelPath, string sysName,string AaPath, string screens="",string marquees="")

        {


            List<AAItems> gamesList = new List<AAItems>();



            foreach (string item in Directory.GetFiles(RomFolder + "\\", "*." + ext))
            {

                string shortRomName = System.IO.Path.GetFileNameWithoutExtension(item);

                string joinedName = shortRomName.Replace(" ", "");
              
               string endPath = System.IO.Path.Combine(AaPath, @"aarcade\library", type, joinedName + type + ".itm");

               gamesList.Add(new AAItems { Title = shortRomName, Type = type, FileLocation = item, App = app, LastModel = modelPath, ScreensLocation = screens + "\\" + shortRomName + ".png", MarqueeLocation = marquees + "\\" + shortRomName + ".png", ItemFileName = endPath });

            }

            createJsonText(gamesList);
        }

    }
}
