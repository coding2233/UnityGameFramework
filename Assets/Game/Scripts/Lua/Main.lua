require('mobdebug').start()
require('Resource')
require('UI')

function Start()
    print("start")
    UI:Open("Assets/Game/UI/Canvas.prefab")
    UI:Close("Assets/Game/UI/Canvas.prefab",false)
    UI:Open("Assets/Game/UI/Canvas.prefab")
    UI:Close("Assets/Game/UI/Canvas.prefab",true)
    UI:Open("Assets/Game/UI/Canvas.prefab")
end

function Update()
    print("lua update...")
end

function Close()
    print("lua close")
end