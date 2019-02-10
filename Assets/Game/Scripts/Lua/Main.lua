require('mobdebug').start()
require('Resource')
require('UI')

function Start()
    print("start")
    UI:Open("Canvas")
end

function Update()
    print("lua update...")
end

function Close()
    print("lua close")
end