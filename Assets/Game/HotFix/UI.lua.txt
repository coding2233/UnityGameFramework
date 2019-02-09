require('Resource')

UI={}

local View={}

-- 打开ui
function UI:Open(name)
  view=View[name]
  if(view==nil) then
    viewPrefab=CS.UnityEngine.Object()
    viewPrefab=Res:LoadAsset(viewPrefab,"ui",name)
    view=CS.UnityEngine.GameObject.Instantiate(viewPrefab)
    view.gameObject:SetActive(true)
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
