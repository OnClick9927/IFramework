---@type IOUtil
IOUtil = {}

--- 检查指定的文件或目录是否存在，如果存在返回 true，否则返回 false
--- @param path string 文件路径
---@return boolean
function IOUtil.Exists(path)
    local file = io.open(path, "r")
    if file then
        io.close(file)
    end
    return file ~= nil
end

--- 读取文件内容，返回包含文件内容的字符串，如果失败返回 nil
--- @param path string 文件路径
---@return string|nil
function IOUtil.ReadText(path)
    local file = io.open(path, "r")
    if file then
        local content = file:read("*a")
        io.close(file)
        return content
    end
    return nil
end

--- 以字符串内容写入文件，成功返回 true，失败返回 false
--- "mode 写入模式" 参数决定 io.writefile() 如何写入内容，可用的值如下：
--- -   "w+" : 覆盖文件已有内容，如果文件不存在则创建新文件
--- -   "a+" : 追加内容到文件尾部，如果文件不存在则创建文件
--- 此外，还可以在 "写入模式" 参数最后追加字符 "b" ，表示以二进制方式写入数据，这样可以避免内容写入不完整。
--- @param path string 文件路径
--- @param content string 内容
--- @param mode string 模式
function IOUtil.WriteText(path, content, mode)
    mode = mode or "w+b"
    local file = io.open(path, mode)
    if file then
        if file:write(content) == nil then
            return false
        end
        io.close(file)
        return true
    else
        return false
    end
end

---拆分一个路径字符串，返回组成路径的各个部分
--- local pathinfo  = io.pathinfo("/var/app/test/abc.png")
--- 结果:
--- pathinfo.dirname  = "/var/app/test/"
--- pathinfo.filename = "abc.png"
--- pathinfo.basename = "abc"
--- pathinfo.extname  = ".png"
--- @param  path string 要分拆的路径字符串
--- @return table
function IOUtil.PathInfo(path)
    local pos = string.len(path)
    local extpos = pos + 1
    while pos > 0 do
        local b = string.byte(path, pos)
        if b == 46 then -- 46 = char "."
            extpos = pos
        elseif b == 47 then -- 47 = char "/"
            break
        end
        pos = pos - 1
    end

    local dirname = string.sub(path, 1, pos)
    local filename = string.sub(path, pos + 1)
    extpos = extpos - pos
    local basename = string.sub(filename, 1, extpos - 1)
    local extname = string.sub(filename, extpos)
    return {
        dirname = dirname,
        filename = filename,
        basename = basename,
        extname = extname
    }
end

-- 返回指定文件的大小，如果失败返回 false
--- @param  path string 文件完全路径
--- @return number
function IOUtil.Filesize(path)
    local size = false
    local file = io.open(path, "r")
    if file then
        local current = file:seek()
        size = file:seek("end")
        file:seek("set", current)
        io.close(file)
    end
    return size
end
