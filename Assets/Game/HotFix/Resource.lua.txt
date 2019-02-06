Res={}

local gameMode=CS.GameFramework.Taurus.GameMode

--@ 加载资源
function Res:LoadAsset(target,ab,path)
  target=gameMode.Resource:LoadAsset(target,ab,path)
  return target
end