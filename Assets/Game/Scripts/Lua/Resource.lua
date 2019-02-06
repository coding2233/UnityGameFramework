Res={}

local gameMode=CS.GameFramework.Taurus.GameMode

function Res:LoadAsset(ab,path)
  local tx=CS.UnityEngine.TextAsset()
  tx=gameMode.Resource:LoadAsset(tx,"hotfix","Assets/Game/HotFix/Test.lua.txt")
  print(tx.text)
  end