require('mobdebug').start()

local Test=require("Test")
local q=10

function Start()
    q=1000
    print("lua start")
    print(q)
	  --CS.GameMode.Resource.Load("hotfix",)
end

function Update()
    print("lua update...")
end

function Close()
    print("lua close")
end