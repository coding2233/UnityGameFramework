
function Enable()
    print("Test--UI--Enable")
end

function Disable()
    print("Test--UI--Disable")
end

function Start()
    print(self)
    TestBtn:GetComponent("Button").onClick:AddListener(ButtonClick)
    print("Test--UI--Start")
end

function Update()
    print("Test--UI--Update")
end

function Close()
    print("Test--UI--Close")
end

function ButtonClick()
  print("button----Click-----------")
end
