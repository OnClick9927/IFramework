local LuaDataType = require("Base.Class.LuaDataType")

---@type TableUtil
TableUtil = {}

--- 深拷贝一个table
---@param t table
---@return table
function TableUtil.DeepCopy(t)
    local SearchTable = {}
    local function Func(object)
        if type(object) ~= LuaDataType.Table then
            return object
        end
        local NewTable = {}
        SearchTable[object] = NewTable
        for k, v in pairs(object) do
            NewTable[Func(k)] = Func(v)
        end
        return setmetatable(NewTable, getmetatable(object))
    end
    return Func(t)
end

--- 清空一个Table
---@param t table
function TableUtil.ClearTable(t)
    for _, v in pairs(t) do
        v = nil
    end
    t = nil
end

--- 计算表格包含的字段数量
--- Lua table 的 "#" 操作只对依次排序的数值下标数组有效，table.nums() 则计算 table 中所有不为 nil 的值的个数。
---@param t table
---@return number
function TableUtil.GetCount(t)
    local count = 0
    for k, v in pairs(t) do
        count = count + 1
    end
    return count
end

--- 返回指定表格中的所有键
--- local hashtable = {a = 1, b = 2, c = 3}
--- local keys = table.keys(hashtable)
--- keys = {"a", "b", "c"}
---@param hashtable table
---@return stringArray
function TableUtil.GetKeys(hashtable)
    local keys = {}
    for k, v in pairs(hashtable) do
        keys[#keys + 1] = k
    end
    return keys
end

--- 返回指定表格中的所有值
--- local hashtable = {a = 1, b = 2, c = 3}
--- local values = table.values(hashtable)
--- values = {1, 2, 3}
---@param hashtable table
---@return ObjectArray
function TableUtil.GetValues(hashtable)
    local values = {}
    for k, v in pairs(hashtable) do
        values[#values + 1] = v
    end
    return values
end

--- 将来源表格中所有键及其值复制到目标表格对象中，如果存在同名键，则覆盖其值
--- local dest = {a = 1, b = 2}
--- local src  = {c = 3, d = 4}
--- table.merge(dest, src)
--- dest = {a = 1, b = 2, c = 3, d = 4}
---@param dest table
---@param src table
function TableUtil.Merge(dest, src)
    for k, v in pairs(src) do
        dest[k] = v
    end
end

--- 在目标表格的指定位置插入来源表格，如果没有指定位置则连接两个表格
--- local dest = {1, 2, 3}
--- local src  = {4, 5, 6}
--- table.Insert(dest, src)
--- dest = {1, 2, 3, 4, 5, 6}

--- dest = {1, 2, 3}
--- table.Insert(dest, src, 5)
--- dest = {1, 2, 3, nil, 4, 5, 6}
---@param dest table
---@param src table
---@param begin number
function TableUtil.Insert(dest, src, begin)
    local bo, begin = Convert.RoundToInt(begin)
    if not bo then
        begin = 0
    end
    if begin <= 0 then
        begin = #dest + 1
    end

    local len = #src
    for i = 0, len - 1 do
        dest[i + begin] = src[i + 1]
    end
end
--- 从表格中查找指定值，返回其索引，如果没找到返回 false
--- local array = {"a", "b", "c"}
--- TableUtil.IndexOf(array, "b")
---输出 2
---@param array table
---@param value any
---@param begin number
function TableUtil.IndexOf(array, value, begin)
    for i = begin or 1, #array do
        if array[i] == value then
            return i
        end
    end
    return false
end

---@param t table
---@param beginIndex any
---@param endIndex number
---@return table
function TableUtil.Copy(t, beginIndex, endIndex)
    local list = {}
    for i, v in ipairs(t) do
        if i >= beginIndex then
            if i <= endIndex then
                table.insert(list, v)
            else
                break
            end
        end
    end

    return list
end

---@param hashtable table
---@param key string
---@return boolean
function TableUtil.ContainsKey(hashtable, key)
    local t = type(hashtable)
    return (t == LuaDataType.Table or t == LuaDataType.UserData) and hashtable[key] ~= nil
end

--- 从表格中删除指定值，返回删除的值的个数
--- local array = {"a", "b", "c", "c"}
--- TableUtil.RemoveValue(array, "c", true) -- 输出 2
---@param array table
---@param value any
---@param removeall boolean
---@return number
function TableUtil.RemoveValue(array, value, removeall)
    local c, i, max = 0, 1, #array
    while i <= max do
        if array[i] == value then
            table.remove(array, i)
            c = c + 1
            i = i - 1
            max = max - 1
            if not removeall then
                break
            end
        end
        i = i + 1
    end
    return c
end

--- 对表格中每一个值执行一次指定的函数，并用函数返回值更新表格内容
---@param t table
---@param func function
function TableUtil.Map(t, func)
    for k, v in pairs(t) do
        t[k] = func(v, k)
    end
end
--- 对表格中每一个值执行一次指定的函数
---@param t table
---@param func function
function TableUtil.Walk(t, func)
    for k, v in pairs(t) do
        func(v, k)
    end
end

--- 对表格中每一个值执行一次指定的函数，如果该函数返回 false，则对应的值会从表格中删除
---@param t table
---@param func function
function TableUtil.Remove(t, func)
    for k, v in pairs(t) do
        if not func(v, k) then
            t[k] = nil
        end
    end
end
-- 遍历表格，确保其中的值唯一
---@param t table
---@param bArray boolean 是否转成数组
---@return table
function TableUtil.Distinct(t, bArray)
    local check = {}
    local n = {}
    local idx = 1
    for k, v in pairs(t) do
        if not check[v] then
            if bArray then
                n[idx] = v
                idx = idx + 1
            else
                n[k] = v
            end
            check[v] = true
        end
    end
    return n
end
