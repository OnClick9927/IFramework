---@class ClassType class类型
local ClassType = {
    --- @field Lua number 纯Lua
    Lua = 0,
    ---@field CreateFirst number 需要先调用__createfirst创建实例对象
    CreateFirst = 1,
    ---@field ExtendCSInstance number 扩展CSharp实例
    ExtendCSInstance = 2
}
return ClassType
