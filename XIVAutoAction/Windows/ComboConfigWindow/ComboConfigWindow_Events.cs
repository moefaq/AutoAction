﻿using ImGuiNET;
using System.Numerics;
using AutoAction.Configuration;
using AutoAction.Localization;
using System.Linq;
using Dalamud.Logging;
using System.Xml.Linq;

namespace AutoAction.Windows.ComboConfigWindow;

internal partial class ComboConfigWindow
{
    private void DrawEvent()
    {
        bool EventsEnabled = Service.Configuration.EnableEvents;
        if (ImGui.Checkbox(LocalizationManager.RightLang.Configwindow_Events_EnableEvent,
            ref EventsEnabled))
        {
            Service.Configuration.EnableEvents = EventsEnabled;
            Service.Configuration.Save();
        }
        ImGui.SameLine();
        Spacing();
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Events_AddEvent))
        {
            Service.Configuration.Events.Add(new ActionEventInfo());
            Service.Configuration.Save();
        }
        ImGui.SameLine();
        Spacing();
        // 新建一个类型，使用默认类型名，没有内置event
        if (ImGui.Button(LocalizationManager.RightLang.Configwindow_Events_AddType))
        {
            Service.Configuration.EventTypes.Add(new ActionEventType());
            Service.Configuration.Save();
        }
        ImGui.SameLine();
        Spacing();
        ImGui.Text(LocalizationManager.RightLang.Configwindow_Events_Description);

        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, new Vector2(0f, 5f));


        if (ImGui.BeginChild("Events List", new Vector2(0f, -1f), true))
        {

            for (int i = 0; i < Service.Configuration.Events.Count(); i++)
            {
                string name = Service.Configuration.Events[i].Name;
                if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionName}##ActionName{i}",
                    ref name, 50))
                {
                    Service.Configuration.Events[i].Name = name;
                    Service.Configuration.Save();
                }
                string eventType = Service.Configuration.Events[i].Type;
                if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionType}##ActionType{i}",
                    ref eventType, 50))
                {
                    Service.Configuration.Events[i].Type = eventType;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_AddType}##AddType{i}"))
                {
                    // 加载在这里的事件本来没有Type，当输入文字之后，需要保存Type
                    // 确认输入的Type是不是在当前已有的Type里
                    bool isTypeExist = false;
                    foreach (var thisType in Service.Configuration.EventTypes.Where(
                    thisType => thisType.TypeName == eventType))
                    {
                        thisType.Events.Add(Service.Configuration.Events[i]);
                        isTypeExist = true;
                        break;
                    }
                    if (!isTypeExist)
                    {
                        // 如果是当前没有的Type，则创建
                        Service.Configuration.EventTypes.Add(new ActionEventType(eventType, Service.Configuration.Events[i]));
                        // 删除当前的event
                        Service.Configuration.Events.RemoveAt(i);
                    }
                    Service.Configuration.Save();
                }
                int macroindex = Service.Configuration.Events[i].MacroIndex;
                if (ImGui.DragInt($"{LocalizationManager.RightLang.Configwindow_Events_MacroIndex}##MacroIndex{i}",
                    ref macroindex, 1, 0, 99))
                {
                    Service.Configuration.Events[i].MacroIndex = macroindex;
                    Service.Configuration.Save();
                }
                string macroString = Service.Configuration.Events[i].macroString;
                if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_MacroString}##MacroString{i}",
                    ref macroString, 200))
                {
                    Service.Configuration.Events[i].macroString = macroString;
                    Service.Configuration.Save();
                }
                bool isEnabled = Service.Configuration.Events[i].IsEnable;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_EnableMacro}##EnableMacro{i}",
                    ref isEnabled))
                {
                    Service.Configuration.Events[i].IsEnable = isEnabled;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                bool isShared = Service.Configuration.Events[i].IsShared;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_ShareMacro}##ShareMacro{i}",
                    ref isShared))
                {
                    Service.Configuration.Events[i].IsShared = isShared;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                bool noMacro = Service.Configuration.Events[i].noMacro;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_NoMacro}##NoMacro{i}",
                    ref noMacro))
                {
                    Service.Configuration.Events[i].noMacro = noMacro;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                bool noCmd = Service.Configuration.Events[i].noCmd;
                if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_NoCmd}##NoCmd{i}",
                    ref noCmd))
                {
                    Service.Configuration.Events[i].noCmd = noCmd;
                    Service.Configuration.Save();
                }
                ImGui.SameLine();
                Spacing();
                if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{i}"))
                {
                    Service.Configuration.Events.RemoveAt(i);
                    Service.Configuration.Save();
                }
                ImGui.Separator();
            }

            ImGui.Separator();
            // 加载带有类型的事件
            try
            {
                for (int i = 0; i < Service.Configuration.EventTypes.Count(); i++)
                {
                    // 折叠分隔
                    if (ImGui.CollapsingHeader(Service.Configuration.EventTypes[i].TypeName))
                    {
                        string TypeName = Service.Configuration.EventTypes[i].TypeName;
                        if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_RenameType}##Rename{i}",
                                ref TypeName, 50))
                        {
                            Service.Configuration.EventTypes[i].RenameType(TypeName);
                            Service.Configuration.Save();
                        }
                        bool isTypeEnabled = Service.Configuration.EventTypes[i].EnableType;
                        // 是否启用
                        if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_EnableMacro}##EnableType{i}",
                            ref isTypeEnabled))
                        {
                            Service.Configuration.EventTypes[i].EnableType = isTypeEnabled;
                            Service.Configuration.Save();
                        }
                        ImGui.SameLine();
                        Spacing();
                        // 添加带有类型的事件
                        if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_AddEvent}##AddEventByType{i}"))
                        {
                            Service.Configuration.EventTypes[i].Events.Add(new ActionEventInfo(Service.Configuration.EventTypes[i].TypeName));
                            Service.Configuration.Save();
                        }
                        ImGui.SameLine();
                        Spacing();
                        if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_DelType}##RemoveType{i}"))
                        {
                            Service.Configuration.EventTypes.RemoveAt(i);
                            Service.Configuration.Save();
                        }
                        ImGui.Separator();
                        // 列出分类的事件

                        for (int j = 0; j < Service.Configuration.EventTypes[i].Events.Count(); j++)
                        {
                            // 二重for，此时访问的event是 Service.Configuration.EventTypes[i].Events[j]
                            string name = Service.Configuration.EventTypes[i].Events[j].Name;
                            if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionName}##ActionName{i}{j}",
                                ref name, 50))
                            {
                                Service.Configuration.EventTypes[i].Events[j].Name = name;
                                Service.Configuration.Save();
                            }
                            // 当某一event已有type，此时更改他的type，发生如下逻辑
                            string eventType = Service.Configuration.EventTypes[i].Events[j].Type;
                            if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_ActionType}##ActionType{i}{j}",
                                ref eventType, 50))
                            {
                                Service.Configuration.EventTypes[i].Events[j].Type = eventType;
                                Service.Configuration.Save();
                            }
                            ImGui.SameLine();
                            Spacing();
                            if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_AddType}##AddType{i}{j}"))
                            {
                                // 加载在这里的事件本来有Type，当输入文字之后，需要添加到别的Type里或者新建Type
                                // 确认输入的Type是不是在当前已有的Type里
                                bool isTypeExist = false;
                                foreach (var thisType in Service.Configuration.EventTypes.Where(
                                thisType => thisType.TypeName == eventType))
                                {
                                    thisType.Events.Add(Service.Configuration.EventTypes[i].Events[j]);
                                    isTypeExist = true;
                                    break;
                                }
                                if (!isTypeExist)
                                {
                                    // 如果是当前没有的Type，则创建
                                    Service.Configuration.EventTypes.Add(new ActionEventType(eventType, Service.Configuration.EventTypes[i].Events[j]));
                                    // 删除当前的event
                                    Service.Configuration.EventTypes[i].Events.RemoveAt(j);
                                }
                                Service.Configuration.Save();
                            }
                            int macroindex = Service.Configuration.EventTypes[i].Events[j].MacroIndex;
                            if (ImGui.DragInt($"{LocalizationManager.RightLang.Configwindow_Events_MacroIndex}##MacroIndex{i}{j}",
                                ref macroindex, 1, 0, 99))
                            {
                                Service.Configuration.EventTypes[i].Events[j].MacroIndex = macroindex;
                                Service.Configuration.Save();
                            }
                            string macroString = Service.Configuration.EventTypes[i].Events[j].macroString;
                            if (ImGui.InputText($"{LocalizationManager.RightLang.Configwindow_Events_MacroString}##MacroString{i}{j}",
                                ref macroString, 200))
                            {
                                Service.Configuration.EventTypes[i].Events[j].macroString = macroString;
                                Service.Configuration.Save();
                            }
                            bool isEnabled = Service.Configuration.EventTypes[i].Events[j].IsEnable;
                            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_EnableMacro}##EnableMacro{i}{j}",
                                ref isEnabled))
                            {
                                Service.Configuration.EventTypes[i].Events[j].IsEnable = isEnabled;
                                Service.Configuration.Save();
                            }
                            ImGui.SameLine();
                            Spacing();
                            bool isShared = Service.Configuration.EventTypes[i].Events[j].IsShared;
                            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_ShareMacro}##ShareMacro{i}{j}",
                                ref isShared))
                            {
                                Service.Configuration.EventTypes[i].Events[j].IsShared = isShared;
                                Service.Configuration.Save();
                            }
                            ImGui.SameLine();
                            Spacing();
                            bool noMacro = Service.Configuration.EventTypes[i].Events[j].noMacro;
                            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_NoMacro}##NoMacro{i}{j}",
                                ref noMacro))
                            {
                                Service.Configuration.EventTypes[i].Events[j].noMacro = noMacro;
                                Service.Configuration.Save();
                            }
                            ImGui.SameLine();
                            Spacing();
                            bool noCmd = Service.Configuration.EventTypes[i].Events[j].noCmd;
                            if (ImGui.Checkbox($"{LocalizationManager.RightLang.Configwindow_Events_NoCmd}##NoCmd{i}{j}",
                                ref noCmd))
                            {
                                Service.Configuration.EventTypes[i].Events[j].noCmd = noCmd;
                                Service.Configuration.Save();
                            }
                            ImGui.SameLine();
                            Spacing();
                            if (ImGui.Button($"{LocalizationManager.RightLang.Configwindow_Events_RemoveEvent}##RemoveEvent{i}{j}"))
                            {
                                // 删除某一type中的event
                                Service.Configuration.EventTypes[i].Events.RemoveAt(j);
                                Service.Configuration.Save();
                            }
                            ImGui.Separator();
                        }

                    }
                }
            }
            catch (System.ArgumentOutOfRangeException)
            {
                PluginLog.Log("发生了已知但是无所谓的错误");
            }

            ImGui.EndChild();
        }
        ImGui.PopStyleVar();

    }
}
