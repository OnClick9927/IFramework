---@class ViewModel:ObservableObject
local ObservableObject = require("Base.Classes.ObservableObject")
local ViewModel = class("ViewModel", ObservableObject)


---@param message Delegate 委托
function ViewModel:ctor(message)
    self.message = message
    self.listenCallback = function(code, ...)
        self.ListenViewEvent(code, ...)
    end
end
---Initialize 初始化
function ViewModel:Initialize()
    self.message:Subscribe(self, self.listenCallback)
    self:OnInitialize()
end
---Dispose 释放
function ViewModel:Dispose()
    self:OnDispose()
    self.message:UnSubscribe(self, self.listenCallback)
    ObservableObject.Dispose(self)
end
--- 获取字段 例子 {number = 666}
--- @return table
function ViewModel:GetFieldTable()
end
---释放回调
function ViewModel:OnDispose()
end

function ViewModel:OnInitialize()
end
---@param code any UIView传入
---@param ... any 委托传入
function ViewModel:ListenViewEvent(code, ...)
end
return ViewModel
