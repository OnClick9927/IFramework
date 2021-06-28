---@type Convert 数据转换
Convert = {}

---ToNumber 对象-》数值
---@param value string
---@param base number
---@return boolean,number
function Convert.ToNumber(value, base)
    local val = tonumber(value, base)
    return val ~= nil, val
end

---RoundToInt 对数值进行四舍五入，如果不是数值则返回 0
---@param value number 数值
---@return boolean,number
function Convert.RoundToInt(value)
    return math.floor(value + 0.5)
end

---AngleToRadian 角度-》弧度
---@param angle number 角度
---@return number
function Convert.AngleToRadian(angle)
    return angle * math.pi / 180
end

---RadianToAngle 弧度转角度
---@param radian number 弧度
---@return number
function Convert.RadianToAngle(radian)
    return radian / math.pi * 180
end

-- "false"-->false ; "ture"-->ture ; other --> nil
---@param value string false/true
---@return boolean,nil
function Convert.ToBool(value)
    if value == "true" then
        return true
    elseif value == "true" then
        return false
    end
end
