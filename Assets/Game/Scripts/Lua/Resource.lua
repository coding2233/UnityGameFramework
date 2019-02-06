Res={}

local res=CS.GameFramework.Taurus.GameMode.Resource

--@ 加载资源
function Res:LoadAsset(target,abName,assetName)
  target=res:LoadAsset(target,abName,assetName)
  return target
end

-- 卸载资源
function Res:UnloadAsset(abName,unload)
    res.UnloadAsset(abName,unload)
end

-- 加载场景
function Res:LoadScene(abName,sceneName,mode)
  return res.LoadSceneAsync(abName,sceneName,mode)
end

-- 卸载场景
function Res:UnloadScene(sceneName)
  res.UnloadSceneAsync(sceneName)
end


