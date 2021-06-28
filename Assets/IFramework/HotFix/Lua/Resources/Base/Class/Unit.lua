local UnitType = require("Base.Class.UnitType")
local LuaDataType = require("Base.Class.LuaDataType")
local ClassType = require("Base.Class.ClassType")

--- @class Unit 元
local Unit = {
    ---@field __unittype UnitType 元类型
    __unittype = UnitType.Unit,
    --if __unittype == UnitType.Type
    ---@field __super Unit 基类型
    __super = nil,
    ---@field __classname string 类型名称
    __classname = "Unit",
    ---@field __classtype ClassType 类型种类
    __classtype = ClassType.Lua,
    ---@field __firstcreate function 创建CS实例  （__classtype = ClassType.CreateFirst 存在）
    __firstcreate = function(...)
    end,
    ---@field ctor function 构造函数
    ctor = function(_, ...)
    end,
    --if __unittype == UnitType.Instance
    ---@field __type Unit 所属类型 ，是一个 {}
    __type = nil,
    --原 CS 对象
    --（__classtype = ClassType.ExtendCSInstance 存在）
    --（__classtype = ClassType.CreateFirst 存在）
    ---@field __object CS.Object
    __object = nil
}

---@param super string|Unit 名字或者一张由 class 创建出的表
---@return boolean @返回 true | false
function Unit:IsSubClassOf(super)
    --- @type Unit
    local _type = nil
    if self.__unittype == UnitType.Instance then
        _type = self.__type
    elseif self.__unittype == UnitType.Type then
        _type = self
    end
    --- @type string
    local classname
    if type(super) == LuaDataType.String then
        classname = super
    elseif type(super) == LuaDataType.Table then
        classname = super.__classname
    end
    local result = false
    local tmp = _type
    while tmp ~= nil do
        if tmp.__classname == classname then
            result = true
            break
        end
        tmp = tmp.__super
    end
    tmp = nil
    classname = nil
    return result
end
return Unit
