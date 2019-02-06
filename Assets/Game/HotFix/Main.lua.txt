require('mobdebug').start()
require('Resource')

function Start()
    print("lua start")
    local tx=CS.UnityEngine.TextAsset()
    tx= Res:LoadAsset(tx,"hotfix","Assets/Game/HotFix/Test.lua.txt");
    print(tx.text)
    CS.UnityEngine.GameObject('lua test gameobject')
end

function Update()
    print("lua update...")
end

function Close()
    print("lua close")
end