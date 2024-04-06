﻿using System;
using System.Collections.Generic;
using System.IO;
using BlueprintEditorPlugin.Editors.BlueprintEditor.Connections;
using BlueprintEditorPlugin.Models.Entities.Networking;
using BlueprintEditorPlugin.Models.Nodes;
using FrostyEditor;

namespace BlueprintEditorPlugin.Editors.BlueprintEditor.Nodes
{
    public class EntityMappingNode : EntityNode
    {
        public static Dictionary<string, string> EntityMappings { get; set; } = new Dictionary<string, string>();
        private StreamReader _reader;

        public void Load(string type)
        {
            Header = Object.GetType().Name; // TODO workaround: ObjectType isn't being assigned. We instead assign it ourselves
            
            _reader = new StreamReader(EntityMappings[type]);
            string line = ReadCleanLine();
            while (line != null)
            {
                if (string.IsNullOrEmpty(line))
                {
                    line = ReadCleanLine();
                    continue;
                }

                try
                {
                    ReadProperty(line);
                }
                catch (Exception e)
                {
                    App.Logger.LogError("Unable to load property {0} in node mapping {1}", line, type);
                }
                
                line = ReadCleanLine();
            }
            
            _reader.Dispose();
        }

        public static void Register()
        {
            foreach (string file in Directory.EnumerateFiles(@"BlueprintEditor\NodeMappings", "*.nmc"))
            {
                try
                {
                    StreamReader reader = new StreamReader(file);
                    string line = reader.ReadLine();
                    if (!line.StartsWith("Type"))
                    {
                        App.Logger.LogError("{0} did not start with the object's type. Please always include the object's type as the first property entry");
                        continue;
                    }
                
                    line = line.Replace(" = ", "=");
            
                    // We split by =" because a property should always look like `property="value"`
                    // Value can be anything, though. Such as "="
                    // If we just split by the char = as a result the value would be split too
                    string[] propertyArg = line.Split(new[] { "=\"" }, StringSplitOptions.None);
                
                    if (EntityMappings.ContainsKey(propertyArg[1].Trim('"')))
                        continue;
                    
                    EntityMappings.Add(propertyArg[1].Trim('"'), file);
                    reader.Dispose();
                }
                catch (Exception e)
                {
                    App.Logger.Log("nmc {0} could not be loaded", file);
                }
            }
        }

        private void ReadProperty(string property)
        {
            // Stupid user input making me have to check everything
            property = property.Replace(" = ", "=");
            
            // We split by =" because a property should always look like `property="value"`
            // Value can be anything, though. Such as "="
            // If we just split by the char = as a result the value would be split too
            string[] propertyArg = property.Split(new[] { "=\"" }, StringSplitOptions.None);
            string value = propertyArg[1];
            value = value.Trim('"');

            switch (propertyArg[0])
            {
                case "Type":
                    break;
                
                case "Header":
                {
                    Header = value;
                } break;
                
                case "InputEvent":
                {
                    AddInput(value, ConnectionType.Event);
                } break;
                case "InputProperty":
                {
                    AddInput(value, ConnectionType.Property);
                } break;
                case "InputLink":
                {
                    AddInput(value, ConnectionType.Link);
                } break;
                
                case "OutputEvent":
                {
                    AddOutput(value, ConnectionType.Event);
                } break;
                case "OutputProperty":
                {
                    AddOutput(value, ConnectionType.Property);
                } break;
                case "OutputLink":
                {
                    AddOutput(value, ConnectionType.Link);
                } break;
            }
        }

        private string ReadCleanLine()
        {
            string line = _reader.ReadLine();
            line = line?.Trim();

            if (!string.IsNullOrEmpty(line))
            {
                int commentPosition = line.IndexOf("//"); //Remove comments
                if (commentPosition != -1)
                {
                    line = line.Remove(commentPosition).Trim();
                }
            }

            return line;
        }
    }
}