require('Resource')

UI={}

local View={}

local uiPrefabPath="Assets/Game/UI/"

-- 打开ui
function UI:Open(name)
  view=View[name]
  if(view==nil) then
    viewPrefab=CS.UnityEngine.Object()
    path=uiPrefabPath..name..".prefab"
    viewPrefab=Res:LoadAsset(viewPrefab,"ui",path)
    view=CS.UnityEngine.GameObject.Instantiate(viewPrefab)
    --如果有LuaBehaviour的脚本则执行相应的脚本
    luaBehaviour=view:GetComponent("GameFramework.Taurus.LuaBehaviour")
    if(luaBehaviour==nil) then
      luaBehaviour:Run(name)
    end
    view:SetActive(true)
    View[name]=view
  else
    view:SetActive(true)
  end
end


--关闭ui
function UI:Close(name,destory)
  view=View[name]
  if(view ~= nil) then
    if(destory) then
      CS.UnityEngine.GameObject.Destroy(view)
      View[name]=nil
    else
      view:SetActive(false)
    end
  end
end
