using ColorLibrary;
using CSharpTest.Net.Collections;
using IndexEngine.Paths;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace IndexEngine.Indexes
{
    public class TagsetIndex: INestedIndex<string,Dictionary<string,Color>, string, Color>
    {
        private static readonly Lazy<TagsetIndex> lazy = new Lazy<TagsetIndex>(() => new TagsetIndex());

        public static TagsetIndex GetInstance()
        {
            return lazy.Value;
        }

        private TagsetIndex() 
        {
            if (!File.Exists(ToolInfo.TagsetColorIndexPath))
            {
                AddDefaultTagset();
            }
            else
            {
                ReadIndexFromDisk();
            }
        }
        public IDictionary<string, Dictionary<string, Color>> IndexCollection { get; private set; } = new BTreeDictionary<string, Dictionary<string, Color>>();

        public int ItemCount { get { return IndexCollection.Count; } }

        private void AddDefaultTagset()
        {
            var list = new List<string> { "JobSearch", "CodeHelp", "FCCBug", "SoftwareSupport", "OSSelection", "Meeting" };
            AddIndexEntry("default", null);
            foreach(var tag in list)
            {
                AddInnerIndexEntry("default", tag, ColorGenerator.GenerateHSLuvColor());
            }
        }

        public bool CheckFiles()
        {
            return (File.Exists(ToolInfo.TagsetColorIndexPath));
        }

        public bool CheckDirectory()
        {
            return (Directory.Exists(ToolInfo.root));
        }

        public void ReadIndexFromDisk()
        {
            if (CheckFiles())
            {
                var jsonString = File.ReadAllText(ToolInfo.TagsetColorIndexPath);
                IndexCollection = JsonConvert.DeserializeObject<BTreeDictionary<string, Dictionary<string,Color>>>(jsonString);
            }
        }

        public void FlushIndexToDisk()
        {
            var jsonString = JsonConvert.SerializeObject(IndexCollection);
            File.WriteAllText(ToolInfo.TagsetColorIndexPath, jsonString);
        }

        public void AddIndexEntry(string key, Dictionary<string, Color> value)
        {
            if (!IndexCollection.ContainsKey(key) && value == null)
            {
                IndexCollection.Add(key, new Dictionary<string, Color>());
            }
            if(!IndexCollection.ContainsKey(key) && value != null)
            {
                IndexCollection.Add(key, value);
            }
        }

        public void AddInnerIndexEntry(string key, string inkey, Color invalue)
        {
            if (IndexCollection.ContainsKey(key))
            {
                if (!IndexCollection[key].ContainsKey(inkey))
                {
                    IndexCollection[key].Add(inkey, invalue);
                }
            }
        }


        public void AddInnerIndexEntry(string key, string inkey)
        {
            if (IndexCollection.ContainsKey(key))
            {
                if (!IndexCollection[key].ContainsKey(inkey))
                {
                    IndexCollection[key].Add(inkey, ColorGenerator.GenerateHSLuvColor());
                }
            }
        }
        public void DeleteIndexEntry(string key)
        {
            if (IndexCollection.ContainsKey(key))
            {
                IndexCollection.Remove(key);
            }
        }

        public void DeleteInnerIndexEntry(string key, string inkey)
        {
            if (IndexCollection.ContainsKey(key))
            {
                if (!IndexCollection[key].ContainsKey(inkey))
                {
                    IndexCollection[key].Remove(inkey);
                }
            }
        }

        public void InitializeIndex(List<string> list)
        {
            foreach(var tag in list)
            {
                AddIndexEntry(tag, null);
            }
        }

        public void UpdateIndexEntry(string key, Dictionary<string, Color> value)
        {
            throw new NotImplementedException();
        }

        public int GetValueCount(string key)
        {
            if (IndexCollection.ContainsKey(key))
            {
                return IndexCollection[key].Count;
            }
            else
            {
                return -1;
            }
        }

        public int GetInnerValueCount(string key, string inkey)
        {
            if (IndexCollection.ContainsKey(key))
            {
                return IndexCollection[key].Count;
            }
            else { return -1; }
        }

        public void UnloadData()
        {
            IndexCollection.Clear();
        }
    }
}