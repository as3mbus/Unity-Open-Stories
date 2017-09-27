using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using LitJson;

namespace as3mbus.Story
{
    public enum fadeMode
    {
        none, color, transition
    }
    public class Phase
    {

        public string name;
        public Comic comic;
        public List<int> pages = new List<int>();
        public List<Vector3> paths = new List<Vector3>();
        public List<float> zooms = new List<float>();
        public List<float> shake = new List<float>();
        public List<Vector3> baloonpos = new List<Vector3>();
        public List<float> baloonsize = new List<float>();
        public List<string> characters = new List<string>();
        public List<string> messages = new List<string>();
        public List<string> animations = new List<string>();
        public List<fadeMode> fademode = new List<fadeMode>();
        public Phase()
        {
            this.name = "";
            this.comic = new Comic("sample comic");
        }
        public Phase(string name, string bundleName, string comicPath)
        {
            this.name = name;
            this.comic = new Comic(bundleName,comicPath);
        }
        public Phase(JsonData chara, JsonData msg)
        {
            for (int i = 0; i < chara.Count; i++)
            {
                this.characters.Add(chara[i].ToString());
                this.messages.Add(msg[i].ToString());
            }
        }

        public Phase(JsonData phaseData)
        {
            try
            {
                this.name = phaseData["name"].ToString();
                this.comic = new Comic(phaseData["comic"]);
                for (int i = 0; i < phaseData["message"].Count; i++)
                {

                    this.pages.Add((int)phaseData["page"][i]);
                    this.paths.Add(
                        new Vector3(
                            float.Parse(phaseData["camx"][i].ToString()),
                            float.Parse(phaseData["camy"][i].ToString()),
                            -10f
                        )
                    );
                    this.baloonpos.Add(
                        new Vector3(
                            float.Parse(phaseData["baloonx"][i].ToString()),
                            float.Parse(phaseData["baloony"][i].ToString()),
                            -1
                        )
                    );
                    this.baloonsize.Add(float.Parse(phaseData["baloonsize"][i].ToString()));
                    Debug.Log("Testing " + paths[i].ToString());
                    this.zooms.Add(float.Parse(phaseData["zoom"][i].ToString()));
                    this.characters.Add(phaseData["character"][i].ToString());
                    this.messages.Add(phaseData["message"][i].ToString());
                    switch (phaseData["fademode"][i].ToString())
                    {
                        case "transition":
                            this.fademode.Add(fadeMode.transition);
                            break;
                        case "color":
                            this.fademode.Add(fadeMode.color);
                            break;
                        default:
                            this.fademode.Add(fadeMode.none);
                            break;
                    }
                }
            }
            catch (System.Exception)
            {
                for (int i = 0; i < phaseData["message"].Count; i++)
                {
                    this.characters.Add(phaseData["character"][i].ToString());
                    this.messages.Add(phaseData["message"][i].ToString());
                }
                throw;
            }

        }

        public string toJson()
        {
            StringBuilder sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(sb);
            writer.PrettyPrint = true;
            writer.IndentValue = 4;
            toJson(writer);
            return sb.ToString();
        }

        public void toJson(JsonWriter writer)
        {

            writer.WriteObjectStart();
            writer.WritePropertyName("name");
            writer.Write(this.name);
            writer.WritePropertyName("comic");
            writer.Write(this.comic.toString());
            writer.WritePropertyName("page");
            writer.WriteArrayStart();
            foreach (var item in this.pages)
            {
                writer.Write(item);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("camx");
            writer.WriteArrayStart();
            foreach (var item in this.paths)
            {
                writer.Write(item.x);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("camy");
            writer.WriteArrayStart();
            foreach (var item in this.paths)
            {
                writer.Write(item.y);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("zoom");
            writer.WriteArrayStart();
            foreach (var item in this.zooms)
            {
                writer.Write(item);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("character");
            writer.WriteArrayStart();
            foreach (var item in this.characters)
            {
                writer.Write(item);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("message");
            writer.WriteArrayStart();
            foreach (var item in this.messages)
            {
                writer.Write(item);
            }
            writer.WriteArrayEnd();
            writer.WritePropertyName("fademode");
            writer.WriteArrayStart();
            foreach (var item in this.fademode)
            {
                writer.Write(item.ToString("g"));
            }
            writer.WriteArrayEnd();
            //writer.Write(string.Join("", new List<int>(array).ConvertAll(i => i.ToString()).ToArray()));
            // writer.WriteArrayEnd();

            writer.WriteObjectEnd();
        }

        public void newLine()
        {
            this.pages.Add(0);
            this.zooms.Add(5f);
            this.paths.Add(new Vector3(0, 0, -10));
            this.characters.Add("");
            this.messages.Add("");
            this.animations.Add("");
        }

        public void deleteLine(int index)
        {
            this.pages.RemoveAt(index);
            this.zooms.RemoveAt(index);
            this.paths.RemoveAt(index);
            this.characters.RemoveAt(index);
            this.messages.RemoveAt(index);
            this.animations.RemoveAt(index);
        }

        public void UpdateLine(string character, string message, int pageNo, float zoom, Vector3 path, int index)
        {
            this.pages[index] = pageNo;
            this.zooms[index] = zoom;
            this.paths[index] = path;
            this.characters[index] = character;
            this.messages[index] = message;
            Debug.Log(this.toJson());
        }

        public void insertLine(int index)
        {
            this.pages.Insert(index, 0);
            this.zooms.Insert(index, 5f);
            this.paths.Insert(index, new Vector3(0, 0, -10));
            this.characters.Insert(index, "");
            this.messages.Insert(index, "");
            this.animations.Insert(index, "");
        }
    }
}