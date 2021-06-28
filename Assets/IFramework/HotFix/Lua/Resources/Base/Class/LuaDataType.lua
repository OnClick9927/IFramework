--- @class LuaDataType Lua数据类型枚举
local LuaDataType = {
    --- @field Number string number类型
    Number = "number",
    --- @field String string string类型
    String = "string",
    --- @field Nil string nil类型
    Nil = "nil",
    --- @field Booean string boolean类型
    Booean = "boolean",
    --- @field Function string function类型
    Function = "function",
    --- @field Table string table类型
    Table = "table",
    --- @field UserData string userdata类型
    UserData = "userdata",
    --- @field Thread string thread类型
    Thread = "thread"
}
return LuaDataType
