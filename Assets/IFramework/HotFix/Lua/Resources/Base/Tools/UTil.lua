_G.DefineTable = {}
_G.UsingTable = {}
_G.UsingStaticTable = {}
---锁住 _G
function Lock_G()
    if _G.__locked then
        return
    end
    local meta = {}
    meta.__newindex = function(_, k, v)
        error("attempt to add a new value to global,key: " .. k, 2)
    end
    meta.__index = function(_, k)
        return rawget(_, k) or rawget(_.DefineTable, k) or rawget(_.UsingStaticTable, k) or rawget(_.UsingTable, k)
    end
    _G.__locked = true
    setmetatable(_G, meta)
end

--- 定义全局变量
--- @param name string 名字
--- @param value any 值
--- @return any value
function Define(name, value)
    rawset(_G.DefineTable, name, value)
    return value
end
-- 是否定义全局变量
--- @param name string 名字
--- @return boolean,any
function IsDefine(name)
    local result = rawget(_G.DefineTable, name)
    return result ~= nil, result
end

local function LoadCSType(name)
    return load("return " .. name)()
end
-- C# 调用方法
-- Using("UnityEngine.KeyCode")
-- print(KeyCode.Space)
--- @param classname string 类型名称
--- @param variant string 变体，用于处理名字冲突
--- @return CS.Type
function Using(classname, variant)
    if not variant then
        local a, b, c = string.find(classname, "[^%s]+%.([^%s]+)")
        if c then
            variant = c
        else
            variant = classname
        end
    end
    local _target = rawget(_G.UsingTable, variant)
    if _target == nil then
        _target = LoadCSType("CS." .. classname)
        rawset(_G.UsingTable, variant, _target)
    end
    return _target
end
-- C# 调用方法
-- local KeyCode= Using("UnityEngine.KeyCode")
-- print(KeyCode.Space)
--- @param classname string 类型名称
--- @return CS.Type
function StaticUsing(classname)
    local _target = rawget(_G.UsingStaticTable, classname)
    if _target == nil then
        _target = LoadCSType("CS." .. classname)
        rawset(_G.UsingStaticTable, classname, _target)
    end
    return _target
end
-- 生成方法句柄
---@param obj any
--- @param method function 类型名称
--- @return function
function Handler(obj, method)
    return function(...)
        return method(obj, ...)
    end
end
