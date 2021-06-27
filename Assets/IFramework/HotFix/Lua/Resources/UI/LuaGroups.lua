local VVMGroup = require("UI.VVMGroup")
local Delegate = require("Base.Classes.Delegate")
---@type LuaGroups
local LuaGroups = class("LuaGroups")
local __CSGroups = StaticUsing("IFramework.Hotfix.Lua.LuaGroups")
function LuaGroups:ctor()
    self.onDispose = Handler(self, self.OnDispose)
    self.onSubscribe = Handler(self, self.OnSubscribe)
    self.onUnSubscribe = Handler(self, self.OnUnSubscribe)
    self.onFindPanel = Handler(self, self.OnFindPanel)
    self.onPress = Handler(self, self.OnPress)
    self.onTop = Handler(self, self.OnTop)
    self.onPop = Handler(self,self.OnPop)
    self.onShow = Handler(self, self.OnShow)
    self.onHide = Handler(self, self.OnHide)
    self.onPause = Handler(self, self.OnPause)
    self.onUnPause = Handler(self, self.OnUnPause)
    self.onClose = Handler(self, self.OnClose)
    self.groups = {}
end

---map: { { Name = "**",ViewType =require("****"), VMType=require("***")},}
--- @param map table 自动生成的
--- @return CS.IFramework.Hotfix.Lua.LuaGroups
function LuaGroups:SetMap(map)
    if map == nil then
        error("map could not be null ")
        return
    end
    self.CS_instance = __CSGroups()
    self.CS_instance:onDispose("+", self.onDispose)
    self.CS_instance:onSubscribe("+", self.onSubscribe)
    self.CS_instance:onUnSubscribe("+", self.onUnSubscribe)
    self.CS_instance:onFindPanel("+", self.onFindPanel)

    self.CS_instance:onPress("+", self.onPress)
    self.CS_instance:onPop("+", self.onPop)
    self.CS_instance:onTop("+", self.onTop)

    self.CS_instance:onShow("+", self.onShow)
    self.CS_instance:onHide("+", self.onHide)
    self.CS_instance:onPause("+", self.onPause)
    self.CS_instance:onUnPause("+", self.onUnPause)
    self.CS_instance:onClose("+", self.onClose)
    self.map = map
    return self.CS_instance
end

-- 以下方法私有

function LuaGroups:OnDispose()
    for i, group in pairs(self.groups) do
        group.view:OnClear()
        group:Dispose()
    end

    self.CS_instance:onDispose("-", self.onDispose)
    self.CS_instance:onSubscribe("-", self.onSubscribe)
    self.CS_instance:onUnSubscribe("-", self.onUnSubscribe)
    self.CS_instance:onFindPanel("-", self.onFindPanel)
    self.CS_instance:onPress("-", self.onPress)
    self.CS_instance:onPop("-", self.onPop)
    self.CS_instance:onTop("-", self.onTop)
    self.CS_instance:onShow("-", self.onShow)
    self.CS_instance:onHide("-", self.onHide)
    self.CS_instance:onPause("-", self.onPause)
    self.CS_instance:onUnPause("-", self.onUnPause)
    self.CS_instance:onClose("-", self.onClose)
    self.groups = nil
    self.CS_instance = nil
    self.map = nil
end
function LuaGroups:FindGroup(name)
    return rawget(self.groups, name)
end
function LuaGroups:OnSubscribe(panel)
    local name = panel.name
    if rawget(self.groups, panel.name) ~= nil then
        print("same name with panel  " .. panel.name)
        return false
    end
    local vvmType

    for i, v in pairs(self.map) do
        if v.Name == name then
            vvmType = v
            break
        end
    end
    if (vvmType == nil) then
        error("not find vvm type with Name :" .. name)
        return false
    end

    local message = Delegate()
    local viewModel = vvmType.VMType(message)
    local view = vvmType.ViewType(message, viewModel, panel)
    local vvmGroup = VVMGroup(panel, view, viewModel, message)
    rawset(self.groups, panel.name, vvmGroup)
end
function LuaGroups:OnUnSubscribe(panel)
    local group = rawget(self.groups, panel.name)
    if group ~= nil then
        group.view:OnClear()
        group:Dispose()
        rawset(self.groups, panel.name, nil)
        return true
    end
    return false
end

function LuaGroups:OnFindPanel(name)
    local group = self:FindGroup(name)
    if group ~= nil then
        return group.panel
    end
    return nil
end
function LuaGroups:OnTop(name, arg)
    self:FindGroup(name).view:OnTop(arg)
end
function LuaGroups:OnPress(name, arg)
    self:FindGroup(name).view:OnPress(arg)
end
function LuaGroups:OnPop(name, arg)
    self:FindGroup(name).view:OnPop(arg)
end

function LuaGroups:OnShow(name)
    self:FindGroup(name).view:OnShow()
end

function LuaGroups:OnHide(name)
    self:FindGroup(name).view:OnHide()
end

function LuaGroups:OnPause(name)
    self:FindGroup(name).view:OnPause()
end

function LuaGroups:OnUnPause(name)
    self:FindGroup(name).view:OnUnPause()
end

function LuaGroups:OnClose(name)
    self:FindGroup(name).view:OnClose()
end
return LuaGroups
