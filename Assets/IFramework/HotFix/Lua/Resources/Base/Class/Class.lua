local UnitType = require("Base.Class.UnitType")
local Unit = require("Base.Class.Unit")
local LuaDataType = require("Base.Class.LuaDataType")
local ClassType = require("Base.Class.ClassType")

---创建一个类
---@param classname string  类名
---@param super table|userdata|nil 父类
---@return table
local class = function(classname, super)
    assert(type(classname) == LuaDataType.String and #classname > 0)

    local superType = type(super)
    --判断是否是C#类
    local isCSharpType = super and superType == LuaDataType.Table and typeof(super)
    --判断是否为C#实例
    local isCSharpInstance = super and superType == LuaDataType.UserData

    local cls = {}
    cls.__unittype = UnitType.Type
    cls.__classname = classname
    if isCSharpInstance and super.__classtype == ClassType.ExtendCSInstance then
        --不允许多次扩展一个C#的实例
        error('the super is not supported in the "class()" function,cannot extends a c# instance multiple times.')
    end

    if isCSharpInstance then
        --直接扩展一个C#的实例
        print("__________isCSharpInstance")
        cls.__classtype = ClassType.ExtendCSInstance
        cls.__unittype = UnitType.Instance
        cls.__type = Unit
        cls.__object = super
        local meta = {
            __call = function(_, ...)
                error(_.__classname .. " is a instance extend from cs instance ")
            end,
            __index = function(_t, k)
                local selffield = rawget(_t, k)
                if selffield then
                    return selffield
                else
                    local fromcs = _t.__object[k]
                    if type(fromcs) == LuaDataType.Function then
                        return function(...)
                            fromcs(_t.__object, ...)
                        end
                    else
                        return fromcs
                    end
                end
            end,
            __newindex = function(_t, k, v)
                local valuetype = type(v)
                if valuetype == LuaDataType.Function then
                    rawset(_t, k, v)
                else
                    if _t.__object[k] then
                        _t.__object[k] = v
                    else
                        rawset(_t, k, v)
                    end
                end
            end
        }
        setmetatable(cls, meta)
        return cls
    elseif (isCSharpType and not super.__classtype) or superType == LuaDataType.Function then
        -- 通过传入C#类型的方式或者通过传入C#类创建函数的方式，继承C#的类，包括静态类
        -- print("__________isCSharpType")
        cls.__super = Unit
        cls.__unittype = UnitType.Type
        cls.__classtype = ClassType.CreateFirst
        if not cls.ctor then
            cls.ctor = function(...)
            end
        end
        local meta = {
            __index = super,
            __call = function(_, ...)
                local object = _.__firstcreate(...)
                local instance = {}
                instance.__object = object
                instance.__type = _
                instance.__unittype = UnitType.Instance
                setmetatable(
                    instance,
                    {
                        __index = function(_t, k)
                            local selffield = rawget(_t, k)
                            if selffield then
                                return selffield
                            else
                                local fromcs = _t.__object[k]
                                if type(fromcs) == LuaDataType.Function then
                                    return function(...)
                                        fromcs(_t.__object, ...)
                                    end
                                else
                                    return fromcs
                                end
                            end
                        end,
                        __newindex = function(_t, k, v)
                            -- print("__newindex",k,v)
                            local valuetype = type(v)
                            if valuetype == LuaDataType.Function then
                                rawset(_t, k, v)
                            else
                                if _t.__object[k] then
                                    _t.__object[k] = v
                                else
                                    rawset(_t, k, v)
                                end
                            end
                        end,
                        __call = function(_, ...)
                            error("this is a Instance of " .. _.__classname)
                        end
                    }
                )
                local tmp = _
                local supers = {}
                while tmp ~= nil do
                    table.insert(supers, 1, tmp)
                    tmp = tmp.__super
                end
                for keysuper, superItem in pairs(supers) do
                    for k, v in pairs(superItem) do
                        if k ~= "__firstcreate" and k ~= "__unittype" then
                            instance[k] = v
                        end
                    end
                end
                local ctorTable = {}
                local tmp = _
                while tmp ~= nil do
                    table.insert(ctorTable, 1, tmp)
                    tmp = tmp.__super
                end

                for k, v in pairs(ctorTable) do
                    local ctor = rawget(v, "ctor")
                    if ctor then
                        ctor(instance, ...)
                    end
                end

                return instance
            end
        }
        if isCSharpType and not super.__classtype then
            cls.__firstcreate = function(...)
                return super(...)
            end
        elseif superType == LuaDataType.Function then
            cls.__firstcreate = super
        end
        setmetatable(cls, meta)
        return cls
    elseif super and super.__classtype == ClassType.CreateFirst then
        -- print("___________________" .. super.__classname)
        -- 继承C#类
        cls.__super = super
        cls.__classtype = ClassType.CreateFirst
        cls.__firstcreate = super.__firstcreate
        local meta = {__index = super}
        meta.__call = function(_, ...)
            local object = _.__firstcreate(...)
            local instance = {}
            instance.__object = object
            instance.__unittype = UnitType.Instance
            instance.__type = _

            setmetatable(
                instance,
                {
                    __index = _,
                    __call = function(_, ...)
                        error("this is a Instance of " .. _.__classname)
                    end,
                    __index = function(_t, k)
                        local selffield = rawget(_t, k)
                        if selffield then
                            return selffield
                        else
                            local fromcs = _t.__object[k]
                            if type(fromcs) == LuaDataType.Function then
                                return function(...)
                                    fromcs(_t.__object, ...)
                                end
                            else
                                return fromcs
                            end
                        end
                    end,
                    __newindex = function(_t, k, v)
                        -- print("__newindex",k,v)
                        local valuetype = type(v)
                        if valuetype == LuaDataType.Function then
                            rawset(_t, k, v)
                        else
                            if _t.__object[k] then
                                _t.__object[k] = v
                            else
                                rawset(_t, k, v)
                            end
                        end
                    end
                }
            )

            local tmp = _
            local supers = {}
            while tmp ~= nil do
                table.insert(supers, 1, tmp)
                tmp = tmp.__super
            end

            for keysuper, superItem in pairs(supers) do
                for k, v in pairs(superItem) do
                    if k ~= "__firstcreate" and k ~= "__unittype" then
                        instance[k] = v
                    end
                end
            end

            local ctorTable = {}
            local tmp = _
            while tmp ~= nil do
                table.insert(ctorTable, 1, tmp)
                tmp = tmp.__super
            end
            for k, v in pairs(ctorTable) do
                local ctor = rawget(v, "ctor")
                if ctor then
                    ctor(instance, ...)
                end
            end
            return instance
        end
        setmetatable(cls, meta)
        return cls
    else
        -- print("__________Lua")

        cls.__classtype = ClassType.Lua
        local meta = {
            __call = function(_, ...)
                local instance = {}
                instance.__unittype = UnitType.Instance
                instance.__type = _
                setmetatable(
                    instance,
                    {
                        __index = _,
                        __call = function(_, ...)
                            error("this is a Instance of " .. _.__classname)
                        end
                    }
                )
                local ctorTable = {}
                local tmp = _
                while tmp ~= nil do
                    table.insert(ctorTable, 1, tmp)
                    tmp = tmp.__super
                end
                for k, v in pairs(ctorTable) do
                    local ctor = rawget(v, "ctor")
                    if ctor then
                        ctor(instance, ...)
                    end
                end
                return instance
            end
        }
        if super == nil then
            cls.__super = Unit
            meta.__index = Unit
        else
            meta.__index = super
            cls.__super = super
        end
        setmetatable(cls, meta)
        return cls
    end
end
return class
