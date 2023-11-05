﻿using System.Collections.ObjectModel;
using BlueprintEditorPlugin.Models.Connections;

namespace BlueprintEditorPlugin.Models.Types.NodeTypes.Shared.Entity
{
    public class AndEntityData : NodeBaseModel
    {
        public override string Name { get; set; } = "AND";
        public override string ObjectType { get; } = "AndEntityData";

        public override ObservableCollection<InputViewModel> Inputs { get; set; } = new ObservableCollection<InputViewModel>();

        public override ObservableCollection<OutputViewModel> Outputs { get; set; } =
            new ObservableCollection<OutputViewModel>
            {
                new OutputViewModel() { Title = "Out", Type = ConnectionType.Property }
            };

        public override void OnCreation()
        {
            if ((int)Object.InputCount == 0) return;
            
            for (int i = 1; i != (int)Object.InputCount; i++)
            {
                Outputs.Add(new OutputViewModel() {Title = $"In{i}", Type = ConnectionType.Property});
            }
        }
        
        public override void OnModified() => OnCreation();
    }
}