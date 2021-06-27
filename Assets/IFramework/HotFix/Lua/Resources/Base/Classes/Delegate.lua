---@type Delegate 委托
local Delegate = class("Delegate")

function Delegate:ctor()
    self._callList = {}
end

---Subscribe 注册监听
---@param object any 监听对象
---@param method function 监听对象方法
function Delegate:Subscribe(object, method)
    local _key = {obj = object, func = method}
    local _call = Handler(object, method)
    rawset(self._callList, _key, _call)
end
---Subscribe 移除注册监听
---@param object any 监听对象
---@param method function 监听对象方法
function Delegate:UnSubscribe(object, method)
    local _key
    for k, v in pairs(self._callList) do
        if (k.obj == object and k.func == method) then
            _key = k
            break
        end
    end
    if (_key ~= nil) then
        self._callList[_key] = nil
    end
end

---Invoke 触发监听
---@param ... any 参数
function Delegate:Invoke(...)
    for k, v in pairs(self._callList) do
        v(...)
    end
end

---Dispose 释放 监听
function Delegate:Dispose()
    TableUtil.ClearTable(self._callList)
end

return Delegate
