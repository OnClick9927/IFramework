---@class UIView
local UIView = class("UIView")


---@param message Delegate 委托
---@param context ViewModel ViewModel 
---@param panel CS.IFramework.UI.UIPanel UI
function UIView:ctor(message, context, panel)
    self.message = message
    self.context = context
    self.panel = panel
    self:OnLoad()
    self:BindProperty()
end

--- @type 绑定对应ViewModel的字段
---@param field string 字段名字
---@param func function 回调
function UIView:BindContextField(field, func)
    local handle = Handler(self, func)
    self.context:Subscribe(field, handle)
end

--- @type 发布一个事件给对应的ViewModel
---@param code any 用于区分的事件编号
---@param ... any 其他参数
function UIView:PublishViewEvent(code, ...)
    self.message:Invoke(code, ...)
end
--- 以下是查找物体

--- @type 根据指定路径查找组件
---@param path string 相对路径
---@param type Component 组件类型
function UIView:GetComponent(path, type)
    local transform = self.panel.transform:Find(path)
    if not transform then
        error("can't find transfom with " .. path .. " in panel " .. self.panel.name)
    end
    return transform:GetComponent(type) or transform:GetComponentInChildren(type, true)
end

--- 以下是绑定ui事件

--- @type 绑定对应Slider的onValueChanged方法
---@param Slider Slider 对应的Slider
---@param func function 回调
function UIView:BindSlider(Slider, func)
    local handle = Handler(self, func)
    Slider.onValueChanged:AddListener(handle)
end

--- @type 取消绑定对应Slider的onValueChanged方法
---@param Slider Slider 对应的Slider
function UIView:UnBindToggle(Slider)
    Slider.onValueChanged:RemoveAllListeners()
    Slider.onValueChanged:Invoke()
end

--- @type 绑定对应Toggle的onValueChanged方法
---@param Toggle Toggle 对应的Toggle
---@param func function 回调
function UIView:BindToggle(Toggle, func)
    local handle = Handler(self, func)
    Toggle.onValueChanged:AddListener(handle)
end

--- @type 取消绑定对应Toggle的onValueChanged方法
---@param Toggle Toggle 对应的Toggle
function UIView:UnBindToggle(Toggle)
    Toggle.onValueChanged:RemoveAllListeners()
    Toggle.onValueChanged:Invoke()
end

--- @type 绑定对应InputField的onValueChanged方法
---@param InputField InputField 对应的InputField
---@param func function 回调
function UIView:BindInputField(InputField, func)
    local handle = Handler(self, func)
    InputField.onValueChanged:AddListener(handle)
end

--- @type 取消绑定对应InputField的onValueChanged方法
---@param InputField InputField 对应的InputField
function UIView:UnBindInputField(InputField)
    InputField.onValueChanged:RemoveAllListeners()
    InputField.onValueChanged:Invoke()
end

--- @type 绑定对应InputField的onEndEdit方法
---@param InputField InputField 对应的InputField
---@param func function 回调
function UIView:BindOnEndEdit(InputField, func)
    local handle = Handler(self, func)
    InputField.onEndEdit:AddListener(handle)
end

--- @type 取消绑定对应InputField的onEndEdit方法
---@param InputField InputField 对应的InputField
function UIView:UnBindOnEndEdit(InputField)
    InputField.onEndEdit:RemoveAllListeners()
    InputField.onEndEdit:Invoke()
end

--- @type 绑定对应InputField的OnValidateInput方法
---@param InputField InputField 对应的InputField
---@param func function 回调
function UIView:BindOnValidateInput(InputField, func)
    local handle = Handler(self, func)
    InputField.onValidateInput = handle
end

--- @type 取消绑定对应InputField的OnValidateInput方法
---@param InputField InputField 对应的InputField
function UIView:UnBindOnValidateInput(InputField)
    InputField.onValidateInput = nil
end

--- @type 绑定对应Button的onClick方法
---@param btn Button 对应的Button
---@param func function 回调
function UIView:BindButton(btn, func)
    local handle = Handler(self, func)
    btn.onClick:AddListener(handle)
end

--- 以下是绑定ui事件
--- @type 取消绑定对应Button的onClick方法
---@param btn Button 对应的Button
function UIView:UnBindButton(btn)
    btn.onClick:RemoveAllListeners()
    btn.onClick:Invoke()
end

--- 以下方法来自UIPanel

function UIView:Show()
    self.panel:Show()
end
function UIView:Hide()
    self.panel:Hide()
end
function UIView:Pause()
    self.panel:Pause()
end
function UIView:UnPause()
    self.panel:UnPause()
end
function UIView:Close()
    self.panel:Close()
end

--- 以下是回调

function UIView:BindProperty()
end
function UIView:Dispose()
end
function UIView:OnLoad()
end
function UIView:OnTop(arg)
end
function UIView:OnPress(arg)
end
function UIView:OnPop(arg)
end
function UIView:OnClear()
end

function UIView:OnShow()
end

function UIView:OnHide()
end

function UIView:OnPause()
end

function UIView:OnUnPause()
end

function UIView:OnClose()
end

return UIView
