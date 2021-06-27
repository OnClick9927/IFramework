require("Base.Tools.UTil")
require("Base.Tools.Log")
require("Base.Tools.TableUtil")
require("Base.Tools.Convert")
require("Base.Tools.IOUtil")
require("Base.Tools.StringUtil")
class = require("Base.Class.Class")
Using("UnityEngine")
Using("IFramework.Launcher")
Json = require("Base.Tools.Json")
local Delegate = require("Base.Classes.Delegate")
local updateEvent = Delegate()
local disposeEvent = Delegate()
local onFixUpdate = Delegate()
local onLateUpdate = Delegate()
local onApplicationFocus = Delegate()
local onApplicationPause = Delegate()

function Awake()
    require("GlobalDefine")
    Lock_G()
    Define("Game", Launcher.instance.game)

    Launcher.env:BindUpdate(Update)
    Launcher.BindFixedUpdate(FixUpdate)
    Launcher.BindLateUpdate(LateUpdate)
    Launcher.BindOnApplicationFocus(OnApplicationFocus)
    Launcher.BindOnApplicationPause(OnApplicationPause)

    require("FixCsharp")
    require("GameLogic")
end

function OnDispose()
    updateEvent:Dispose()
    onLateUpdate:Dispose()
    onFixUpdate:Dispose()
    onApplicationFocus:Dispose()
    onApplicationPause:Dispose()
    disposeEvent:Invoke()
    disposeEvent:Dispose()
    Launcher.UnBindFixedUpdate(FixUpdate)
    Launcher.UnBindLateUpdate(LateUpdate)
    Launcher.UnBindOnApplicationFocus(OnApplicationFocus)
    Launcher.UnBindOnApplicationPause(OnApplicationPause)
    Launcher.env:UnBindUpdate(Update)
end

function Update()
    updateEvent:Invoke()
end
function FixUpdate()
    onFixUpdate:Invoke()
end
function LateUpdate()
    onLateUpdate:Invoke()
end
function OnApplicationFocus(foucus)
    onApplicationFocus:Invoke(foucus)
end
function OnApplicationPause(pause)
    onApplicationPause:Invoke(pause)
end

--绑定Unity 回调

function BindToUpdate(object, method)
    updateEvent:Subscribe(object, method)
end
function BindToDispose(object, method)
    disposeEvent:Subscribe(object, method)
end
function BindToFixUpdate(object, method)
    onFixUpdate:Subscribe(object, method)
end
function BindToLateUpdate(object, method)
    onLateUpdate:Subscribe(object, method)
end
function BindToOnApplicationFocus(object, method)
    onApplicationFocus:Subscribe(object, method)
end
function BindToOnApplicationPause(object, method)
    onApplicationPause:Subscribe(object, method)
end

function UnBindToDispose(object, method)
    disposeEvent:UnSubscribe(object, method)
end
function UnBindToUpdate(object, method)
    updateEvent:UnSubscribe(object, method)
end
function UnBindToFixUpdate(object, method)
    onFixUpdate:UnSubscribe(object, method)
end
function UnBindToLateUpdate(object, method)
    onLateUpdate:UnSubscribe(object, method)
end
function UnBindToOnApplicationFocus(object, method)
    onApplicationFocus:UnSubscribe(object, method)
end
function UnBindToOnApplicationPause(object, method)
    onApplicationPause:UnSubscribe(object, method)
end
